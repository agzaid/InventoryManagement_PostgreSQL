using Application.Interfaces.Contracts.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;
using Domain.Entities;

namespace InventoryManagement.Helpers
{
    public static class PermissionHelper
    {
        public static async Task<bool> HasPermissionAsync(this IHtmlHelper htmlHelper, string permissionCode)
        {
            var permissionService = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<IPermissionService>();
            var userManager = htmlHelper.ViewContext.HttpContext.RequestServices.GetService<UserManager<ApplicationUser>>();
            
            if (permissionService == null || userManager == null)
                return false;

            var user = await userManager.GetUserAsync(htmlHelper.ViewContext.HttpContext.User);
            if (user == null)
                return false;

            return await permissionService.HasPermissionAsync(user.Id, permissionCode);
        }
    }
}
