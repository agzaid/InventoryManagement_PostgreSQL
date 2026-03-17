using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Persistance;
using System.Security.Claims;

namespace InventoryManagement.Services
{
    public class CustomUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<ApplicationUser, ApplicationRole>
    {
        private readonly InventoryManagementDbContext _context;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<CustomUserClaimsPrincipalFactory> _logger;

        public CustomUserClaimsPrincipalFactory(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            InventoryManagementDbContext context,
            ILogger<CustomUserClaimsPrincipalFactory> logger)
            : base(userManager, roleManager, optionsAccessor)
        {
            _context = context;
            _roleManager = roleManager;
            _logger = logger;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(ApplicationUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            _logger.LogInformation("Generating claims for user: {UserName}", user.UserName);

            // Get user roles
            var userRoles = await UserManager.GetRolesAsync(user);
            _logger.LogInformation("User {UserName} has {RoleCount} roles: {Roles}", user.UserName, userRoles.Count, string.Join(", ", userRoles));

            // Get permissions for all user roles
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    _logger.LogInformation("Processing role: {RoleName}, IsActive: {IsActive}, RoleId: {RoleId}", roleName, role.IsActive, role.Id);

                    if (role.IsActive)
                    {
                        var rolePermissions = await _context.RolePermissions
                            .Where(rp => rp.RoleId == role.Id && rp.IsAllowed)
                            .Select(rp => rp.PermissionCode)
                            .ToListAsync();

                        _logger.LogInformation("Found {PermissionCount} permissions for role {RoleName}: {Permissions}", 
                            rolePermissions.Count, roleName, string.Join(", ", rolePermissions));

                        foreach (var permission in rolePermissions)
                        {
                            // Add permission claim
                            if (!identity.HasClaim(c => c.Type == "Permission" && c.Value == permission))
                            {
                                identity.AddClaim(new Claim("Permission", permission));
                                _logger.LogInformation("Added permission claim: {Permission}", permission);
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Role {RoleName} is inactive, skipping permissions", roleName);
                    }
                }
                else
                {
                    _logger.LogWarning("Role {RoleName} not found", roleName);
                }
            }

            var permissionClaims = identity.Claims.Where(c => c.Type == "Permission").ToList();
            _logger.LogInformation("Total permission claims added: {Count}", permissionClaims.Count);

            return identity;
        }
    }
}
