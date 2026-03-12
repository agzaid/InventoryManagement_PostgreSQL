using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Application.Services;
using Microsoft.AspNetCore.Http;

namespace InventoryManagement.Middleware
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IJwtService _jwtService;

        public AuthMiddleware(RequestDelegate next, IJwtService jwtService)
        {
            _next = next;
            _jwtService = jwtService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authentication for login page and static files
            var path = context.Request.Path.Value?.ToLower();
            if (path != null && (path.Contains("/account/login") || 
                               path.Contains("/account/accessdenied") ||
                               path.StartsWith("/lib/") ||
                               path.StartsWith("/css/") ||
                               path.StartsWith("/js/") ||
                               path.EndsWith(".css") ||
                               path.EndsWith(".js") ||
                               path.EndsWith(".ico")))
            {
                await _next(context);
                return;
            }

            // Check for token in cookie or header
            var token = context.Request.Cookies["AuthToken"] ?? 
                       context.Request.Headers["Authorization"].FirstOrDefault()?.Replace("Bearer ", "");

            if (!string.IsNullOrEmpty(token))
            {
                var principal = _jwtService.ValidateToken(token);
                if (principal != null)
                {
                    context.User = principal;
                    
                    // Add user info to HttpContext for easy access
                    var userIdClaim = principal.FindFirst(ClaimTypes.NameIdentifier);
                    if (userIdClaim != null && int.TryParse(userIdClaim.Value, out var userId))
                    {
                        context.Items["UserId"] = userId;
                        context.Items["Username"] = principal.FindFirst(ClaimTypes.Name)?.Value;
                        context.Items["Email"] = principal.FindFirst(ClaimTypes.Email)?.Value;
                        context.Items["Roles"] = principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
                    }
                }
                else
                {
                    // Token is invalid, clear it
                    context.Response.Cookies.Delete("AuthToken");
                }
            }

            await _next(context);
        }
    }
}
