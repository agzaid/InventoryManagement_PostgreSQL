using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class LanguageController : Controller
    {
        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            var selectedCulture = culture.StartsWith("ar") ? "ar" : "en";

            // 1. Set Cookie
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(selectedCulture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            // 2. Get the PathBase (e.g., "/inventory")
            var pathBase = Request.PathBase.Value ?? "";

            // 3. Remove PathBase from returnUrl to process the "internal" route
            string remainingPath = returnUrl;
            if (!string.IsNullOrEmpty(pathBase) && returnUrl.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
            {
                remainingPath = returnUrl.Substring(pathBase.Length);
            }

            // 4. Rewrite the internal part (e.g., /en/Home -> /ar/Home)
            string localizedPath = RewriteUrlWithCulture(remainingPath, selectedCulture);

            // 5. Combine them back together
            string finalUrl = pathBase + localizedPath;

            if (Url.IsLocalUrl(finalUrl))
            {
                return LocalRedirect(finalUrl);
            }

            return RedirectToAction("Index", "Home", new { culture = selectedCulture });
        }

        private string RewriteUrlWithCulture(string url, string culture)
        {
            // If empty or just "/", return the culture root
            if (string.IsNullOrEmpty(url) || url == "/")
                return $"/{culture}";

            var parts = url.Split('/', StringSplitOptions.RemoveEmptyEntries).ToList();
            var supported = new[] { "en", "ar" };

            if (parts.Count > 0 && supported.Contains(parts[0]))
            {
                // Change existing: /en/Home -> /ar/Home
                parts[0] = culture;
            }
            else
            {
                // Prepend new: /Home -> /ar/Home
                parts.Insert(0, culture);
            }

            return "/" + string.Join("/", parts);
        }
    }
}
