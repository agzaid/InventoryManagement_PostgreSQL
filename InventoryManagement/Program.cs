using Application;
using Application.Interfaces.Contracts.Localization;
using Infrastructure;
using InventoryManagement.DependencyInjection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Localization.Routing;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Persistance;
using System.Globalization;
using Telerik.Reporting.Cache.File;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddProjectModules();
builder.Services.ConfigureInfrastructureServices();
builder.Services.ConfigureApplicationServices();

builder.Host.ConfigureSerilog();

builder.Services.AddLocalization();

builder.Services.AddScoped<IAppLocalizer, Infrastructure.Localization.AppLocalizer>();

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

app.UseMiddleware<ExceptionHandlingMiddleware>();
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


