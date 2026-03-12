using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Telerik.Reporting.Services;
using Telerik.Reporting.Services.AspNetCore;

namespace InventoryManagement.Controllers
{
    [Route("{culture}/api/reports")]
    [ApiController]
    [Authorize]
    public class ReportsController : ReportsControllerBase
    {
        public ReportsController(IReportServiceConfiguration reportServiceConfiguration)
            : base(reportServiceConfiguration)
        {

        }
    }
}
