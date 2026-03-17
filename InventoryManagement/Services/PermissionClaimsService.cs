using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistance;
using System.Security.Claims;

namespace InventoryManagement.Services
{
    public interface IPermissionClaimsService
    {
        Task<List<Claim>> GetUserPermissionClaimsAsync(ApplicationUser user);
        Task AddPermissionClaimsToUserAsync(ApplicationUser user, SignInManager<ApplicationUser> signInManager);
    }

    public class PermissionClaimsService : IPermissionClaimsService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly InventoryManagementDbContext _context;

        public PermissionClaimsService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            InventoryManagementDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<List<Claim>> GetUserPermissionClaimsAsync(ApplicationUser user)
        {
            var claims = new List<Claim>();
            
            var userRoles = await _userManager.GetRolesAsync(user);
            
            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null && role.IsActive)
                {
                    var rolePermissions = await _context.RolePermissions
                        .Where(rp => rp.RoleId == role.Id && rp.IsAllowed)
                        .Select(rp => rp.PermissionCode)
                        .ToListAsync();

                    foreach (var permission in rolePermissions)
                    {
                        if (!claims.Any(c => c.Type == "Permission" && c.Value == permission))
                        {
                            claims.Add(new Claim("Permission", permission));
                        }
                    }
                }
            }

            return claims;
        }

        public async Task AddPermissionClaimsToUserAsync(ApplicationUser user, SignInManager<ApplicationUser> signInManager)
        {
            var permissionClaims = await GetUserPermissionClaimsAsync(user);
            
            var existingClaims = await _userManager.GetClaimsAsync(user);
            var existingPermissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();

            foreach (var claim in existingPermissionClaims)
            {
                await _userManager.RemoveClaimAsync(user, claim);
            }

            foreach (var claim in permissionClaims)
            {
                await _userManager.AddClaimAsync(user, claim);
            }

            await signInManager.RefreshSignInAsync(user);
        }
    }
}
