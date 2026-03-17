using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Controllers
{
    public class BaseController : Controller
    {
        protected string GetCurrentCulture()
        {
            var culture = RouteData.Values["culture"]?.ToString();
            if (string.IsNullOrEmpty(culture))
            {
                var cookieCulture = Request.Cookies["EGX.Culture"];
                if (!string.IsNullOrEmpty(cookieCulture))
                {
                    var parts = cookieCulture.Split('|');
                    if (parts.Length > 0)
                    {
                        culture = parts[0].Replace("c=", "").Trim();
                    }
                }
            }
            return culture ?? "en";
        }

        protected RedirectToActionResult RedirectToActionWithCulture(string actionName)
        {
            return RedirectToAction(actionName, new { culture = GetCurrentCulture() });
        }

        protected RedirectToActionResult RedirectToActionWithCulture(string actionName, string controllerName)
        {
            return RedirectToAction(actionName, controllerName, new { culture = GetCurrentCulture() });
        }

        protected RedirectToActionResult RedirectToActionWithCulture(string actionName, object routeValues)
        {
            var culture = GetCurrentCulture();
            var values = new RouteValueDictionary(routeValues);
            values["culture"] = culture;
            return RedirectToAction(actionName, values);
        }

        protected RedirectToActionResult RedirectToActionWithCulture(string actionName, string controllerName, object routeValues)
        {
            var culture = GetCurrentCulture();
            var values = new RouteValueDictionary(routeValues);
            values["culture"] = culture;
            return RedirectToAction(actionName, controllerName, values);
        }
    }
}
