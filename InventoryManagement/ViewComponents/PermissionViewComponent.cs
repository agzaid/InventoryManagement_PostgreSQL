using Application.Interfaces.Contracts.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain.Entities;

namespace InventoryManagement.ViewComponents
{
    [ViewComponent(Name = "Permission")]
    public class PermissionViewComponent : ViewComponent
    {
        private readonly IPermissionService _permissionService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PermissionViewComponent(IPermissionService permissionService, UserManager<ApplicationUser> userManager)
        {
            _permissionService = permissionService;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string permissionCode)
        {
            var user = await _userManager.GetUserAsync(User as ClaimsPrincipal);
            if (user == null)
            {
                return View(false);
            }

            var hasPermission = await _permissionService.HasPermissionAsync(user.Id, permissionCode);
            return View(hasPermission);
        }
    }
}
