using Application.Interfaces.Contracts.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;

namespace Infrastructure.Authorization
{
    public class AuthorizePermissionAttribute : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionCode;

        public AuthorizePermissionAttribute(string permissionCode)
        {
            _permissionCode = permissionCode;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            // Skip authorization if action has AllowAnonymous attribute
            if (context.ActionDescriptor.EndpointMetadata
                .Any(em => em is Microsoft.AspNetCore.Authorization.AllowAnonymousAttribute))
            {
                return;
            }

            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new ChallengeResult();
                return;
            }

            // Get the permission service from the service provider
            var permissionService = context.HttpContext.RequestServices.GetService(typeof(IPermissionService)) as IPermissionService;
            if (permissionService == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            var userId = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new ForbidResult();
                return;
            }

            // Check if user has the required permission
            var hasPermission = await permissionService.HasPermissionAsync(userId, _permissionCode);
            if (!hasPermission)
            {
                context.Result = new ForbidResult();
                return;
            }
        }
    }
}
