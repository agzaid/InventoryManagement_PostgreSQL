using Application;
using Application.Interfaces.Contracts.Localization;
using Infrastructure;
using InventoryManagement.DependencyInjection;
using Domain.Entities;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistance;
using System.Globalization;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProjectModules();
builder.Services.ConfigureInfrastructureServices();
builder.Services.ConfigureApplicationServices();

builder.Host.ConfigureSerilog();

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<InventoryManagementDbContext>()
.AddDefaultTokenProviders();

// Configure authentication cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.LogoutPath = "/Account/Logout";
    options.Cookie.Name = "EGX.Identity";
    options.Cookie.HttpOnly = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(1);
    options.SlidingExpiration = true;
});

// Add authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("ManagerOrAbove", policy => policy.RequireRole("Admin", "Manager"));
    options.AddPolicy("DailyMovement", policy => 
        policy.RequireAssertion(context => 
            context.User.IsInRole("Admin") || 
            context.User.IsInRole("Manager") || 
            (context.User.IsInRole("User") && HasPermission(context.User, "PROG01"))));
});

builder.Services.AddLocalization();

builder.Services.AddScoped<IAppLocalizer, Infrastructure.Localization.AppLocalizer>();
builder.Services.AddScoped<InventoryManagement.Services.RoleSeedingService>();

builder.Services.AddControllersWithViews()
    .AddDataAnnotationsLocalization(options => {
        options.DataAnnotationLocalizerProvider = (type, factory) =>
            factory.Create(typeof(Infrastructure.Resources.CommonResource));
    }).AddNewtonsoftJson();

builder.Services.TryAddSingleton<IReportServiceConfiguration>(sp =>
    new ReportServiceConfiguration
    {
        ReportingEngineConfiguration = sp.GetService<IConfiguration>(),
        HostAppId = "InventoryApp",
        Storage = new FileStorage(),
        ReportSourceResolver = new UriReportSourceResolver(
            Path.Combine(builder.Environment.ContentRootPath, "Reports"))
    });
builder.Services.ConfigurePersistanceServices(builder.Configuration);

// ADD THIS - Configure forwarded headers for reverse proxy
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor |
                              ForwardedHeaders.XForwardedProto |
                              ForwardedHeaders.XForwardedHost;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("ar") };

var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures("en", "ar")
    .AddSupportedUICultures("en", "ar");

// Add cookie provider for language persistence
localizationOptions.RequestCultureProviders.Insert(0, new CookieRequestCultureProvider
{
    CookieName = "EGX.Culture"
});

localizationOptions.ApplyCurrentCultureToResponseHeaders = true;

var routeCultureProvider = new RouteDataRequestCultureProvider
{
    RouteDataStringKey = "culture",
    UIRouteDataStringKey = "culture"
};

localizationOptions.RequestCultureProviders.Insert(0, routeCultureProvider);

var app = builder.Build();

// ADD THIS - Use forwarded headers (must be first)
app.UseForwardedHeaders();

// ADD THIS - Use path base from environment variable
var pathBase = Environment.GetEnvironmentVariable("ASPNETCORE_PATHBASE");
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseRequestLocalization(localizationOptions);

// Seed roles and admin user on startup
using (var scope = app.Services.CreateScope())
{
    var roleSeeder = scope.ServiceProvider.GetRequiredService<InventoryManagement.Services.RoleSeedingService>();
    await roleSeeder.SeedRolesAsync();
    await roleSeeder.SeedAdminUserAsync();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "cultureRoute",
    pattern: "{culture}/{controller=Home}/{action=Index}/{id?}",
    constraints: new { culture = "en|ar" }
);
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}",
    defaults: new { culture = "en" }
);

app.Run();

// Helper function for permission checking
bool HasPermission(System.Security.Claims.ClaimsPrincipal user, string permission)
{
    // In a real implementation, you would check user permissions from the database
    // For now, we'll return true for Admin and Manager roles
    return user.IsInRole("Admin") || user.IsInRole("Manager");
}


