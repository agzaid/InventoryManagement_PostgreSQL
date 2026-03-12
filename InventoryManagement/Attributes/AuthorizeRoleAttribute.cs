using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace InventoryManagement.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeRoleAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string[] _roles;

        public AuthorizeRoleAttribute(params string[] roles)
        {
            _roles = roles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Skip if AllowAnonymous attribute is present
            if (context.ActionDescriptor.EndpointMetadata.Any(em => em.GetType() == typeof(AllowAnonymousAttribute)))
            {
                return;
            }

            var user = context.HttpContext.User;
            
            // Check if user is authenticated
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            // Check if user has required role
            if (_roles.Length > 0)
            {
                var userRoles = context.HttpContext.Items["Roles"] as System.Collections.Generic.List<string> ?? 
                               user.Claims.Where(c => c.Type == System.Security.Claims.ClaimTypes.Role)
                                        .Select(c => c.Value)
                                        .ToList();

                if (!_roles.Any(role => userRoles.Contains(role)))
                {
                    context.Result = new RedirectToActionResult("AccessDenied", "Account", null);
                    return;
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AllowAnonymousAttribute : Attribute
    {
    }
}
