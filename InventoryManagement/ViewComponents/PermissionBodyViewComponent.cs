using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using System.Threading.Tasks;

namespace InventoryManagement.ViewComponents
{
    [ViewComponent(Name = "PermissionBody")]
    public class PermissionBodyViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string permissionCode)
        {
            ViewBag.PermissionCode = permissionCode;
            return View();
        }
    }
}
