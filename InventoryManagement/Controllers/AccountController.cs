using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using InventoryManagement.ViewModels;
using Application.Interfaces.Contracts.Persistance;
using System.Security.Claims;

namespace InventoryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            ILogger<AccountController> logger,
            IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                // First, try to authenticate with InvUser
                var invUsers = await _unitOfWork.InvUserRepository.GetAllUsersWithEmployeeDetailsAsync();
                var invUser = invUsers.FirstOrDefault(u => u.UserName == model.Username && u.UserPasswd == model.Password);

                if (invUser != null)
                {
                    // InvUser found - create or update ApplicationUser
                    var appUser = await _userManager.FindByNameAsync(model.Username);
                    
                    if (appUser == null)
                    {
                        // Create new ApplicationUser from InvUser
                        appUser = new ApplicationUser
                        {
                            UserName = invUser.UserName,
                            Email = $"{invUser.UserName}@inventory.EGX",
                            FullName = invUser.Employee?.EmpName ?? invUser.UserName,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        var createResult = await _userManager.CreateAsync(appUser, model.Password);
                        if (!createResult.Succeeded)
                        {
                            ModelState.AddModelError(string.Empty, "Failed to create user account.");
                            return View(model);
                        }
                    }
                    else if (!appUser.IsActive)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is deactivated. Please contact administrator.");
                        return View(model);
                    }

                    // Remove existing permission claims to prevent duplicates
                    var existingClaims = await _userManager.GetClaimsAsync(appUser);
                    var permissionClaims = existingClaims.Where(c => c.Type == "Permission").ToList();
                    if (permissionClaims.Any())
                    {
                        await _userManager.RemoveClaimsAsync(appUser, permissionClaims);
                    }

                    // Add permission claims based on InvUser Prog fields
                    var claims = new List<Claim>();
                    AddPermissionClaim(claims, invUser.Prog01, SystemPermissions.PROG01);
                    AddPermissionClaim(claims, invUser.Prog02, SystemPermissions.PROG02);
                    AddPermissionClaim(claims, invUser.Prog03, SystemPermissions.PROG03);
                    AddPermissionClaim(claims, invUser.Prog11, SystemPermissions.PROG11);
                    AddPermissionClaim(claims, invUser.Prog12, SystemPermissions.PROG12);
                    AddPermissionClaim(claims, invUser.Prog13, SystemPermissions.PROG13);
                    AddPermissionClaim(claims, invUser.Prog14, SystemPermissions.PROG14);
                    AddPermissionClaim(claims, invUser.Prog21, SystemPermissions.PROG21);
                    AddPermissionClaim(claims, invUser.Prog22, SystemPermissions.PROG22);
                    AddPermissionClaim(claims, invUser.Prog23, SystemPermissions.PROG23);
                    AddPermissionClaim(claims, invUser.Prog24, SystemPermissions.PROG24);
                    AddPermissionClaim(claims, invUser.Prog25, SystemPermissions.PROG25);
                    AddPermissionClaim(claims, invUser.Prog29, SystemPermissions.PROG29);
                    AddPermissionClaim(claims, invUser.Prog31, SystemPermissions.PROG31);
                    AddPermissionClaim(claims, invUser.Prog32, SystemPermissions.PROG32);
                    AddPermissionClaim(claims, invUser.Prog33, SystemPermissions.PROG33);
                    AddPermissionClaim(claims, invUser.Prog34, SystemPermissions.PROG34);
                    AddPermissionClaim(claims, invUser.Prog35, SystemPermissions.PROG35);

                    if (claims.Any())
                    {
                        await _userManager.AddClaimsAsync(appUser, claims);
                    }

                    // Sign in the user with updated claims
                    await _signInManager.SignInAsync(appUser, model.RememberMe);

                    // Update last login
                    appUser.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(appUser);

                    _logger.LogInformation("InvUser {UserName} logged in.", model.Username);
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }

                // If not InvUser, try standard ApplicationUser authentication
                var user = await _userManager.FindByNameAsync(model.Username);
                if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View(model);
                }

                if (!user.IsActive)
                {
                    ModelState.AddModelError(string.Empty, "Your account is deactivated. Please contact administrator.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(
                    model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    // Update last login
                    user.LastLoginAt = DateTime.UtcNow;
                    await _userManager.UpdateAsync(user);

                    _logger.LogInformation("User {UserName} logged in.", model.Username);
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }

            return View(model);
        }

        private void AddPermissionClaim(List<Claim> claims, string progValue, string permissionCode)
        {
            if (!string.IsNullOrEmpty(progValue) && (progValue == "1" || progValue.Equals("true", StringComparison.OrdinalIgnoreCase)))
            {
                claims.Add(new Claim("Permission", permissionCode));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FullName = $"{model.FirstName} {model.LastName}",
                    EmployeeCode = model.EmployeeCode,
                    Department = model.Department,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    // Assign default role
                    if (!string.IsNullOrEmpty(model.RoleId))
                    {
                        var role = await _roleManager.FindByIdAsync(model.RoleId);
                        if (role != null)
                        {
                            await _userManager.AddToRoleAsync(user, role.Name!);
                        }
                    }
                    else
                    {
                        // Assign default "User" role
                        await _userManager.AddToRoleAsync(user, "User");
                    }

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    
                    _logger.LogInformation("User created a new account with password.");
                    
                    if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                
                AddErrors(result);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName,
                EmployeeCode = user.EmployeeCode,
                Department = user.Department,
                PhoneNumber = user.PhoneNumber,
                // Address = user.Address, // Will be added later
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                // UpdatedAt = user.UpdatedAt, // Will be added later
                Roles = (await _userManager.GetRolesAsync(user)).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            // Update user properties
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.EmployeeCode = model.EmployeeCode ?? 0;
            user.Department = model.Department;
            user.PhoneNumber = model.PhoneNumber;
            // user.Address = model.Address; // Will be added later
            user.IsActive = model.IsActive;
            // user.UpdatedAt = DateTime.UtcNow; // Will be added later

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Profile updated successfully!";
                return RedirectToAction("Profile");
            }

            AddErrors(result);
            return View(model);
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}
