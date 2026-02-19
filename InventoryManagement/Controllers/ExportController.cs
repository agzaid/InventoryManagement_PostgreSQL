using Microsoft.AspNetCore.Mvc;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Telerik.Reporting.Services;

namespace InventoryManagement.Controllers
{
    [Route("{culture}/api/[controller]")]
    [ApiController]
    public class ExportController : ControllerBase
    {
        private readonly IReportServiceConfiguration reportServiceConfiguration;
        private readonly IHostEnvironment hostEnvironment;

        public ExportController(IReportServiceConfiguration reportServiceConfiguration, IHostEnvironment hostEnvironment)
        {
            this.reportServiceConfiguration = reportServiceConfiguration;
            this.hostEnvironment = hostEnvironment;
        }

        [HttpGet("pdf")]
        public IActionResult Pdf([FromQuery] string report)
        {
            return Render("PDF", "application/pdf", report);
        }

        [HttpGet("xlsx")]
        public IActionResult Xlsx([FromQuery] string report)
        {
            return Render("XLSX", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", report);
        }

        private IActionResult Render(string format, string contentType, string report)
        {
            // 1. Validate Input
            if (string.IsNullOrWhiteSpace(report))
            {
                return BadRequest("Missing required query parameter: report");
            }

            var safeReportName = Path.GetFileName(report);
            var reportsPath = Path.Combine(hostEnvironment.ContentRootPath, "Reports");
            var reportFullPath = Path.Combine(reportsPath, safeReportName);

            if (!System.IO.File.Exists(reportFullPath))
            {
                return NotFound($"Report not found: {safeReportName}");
            }

            // 2. Prepare Parameters from Query String
            var parameters = GetQueryParametersExcept("report");

            if (!parameters.ContainsKey("ReportName"))
            {
                string arabicTitle = "بيان جرد المخزن - ";
                parameters.Add("ReportName", arabicTitle + Path.GetFileNameWithoutExtension(safeReportName));
            }
            ReportSource resolvedReportSource;
            try
            {
                resolvedReportSource = reportServiceConfiguration.ReportSourceResolver.Resolve(
                    safeReportName,
                    OperationOrigin.GenerateReportDocument,
                    parameters
                );

                // 2. CRITICAL STEP: Manually force the parameters into the resolved source
                if (resolvedReportSource is UriReportSource uriSource)
                {
                    foreach (var param in parameters)
                    {
                        var value = param.Value;

                        // Date conversion logic
                        if (param.Key.ToLower().Contains("date") && DateTime.TryParse(value.ToString(), out DateTime parsedDate))
                        {
                            value = parsedDate;
                        }

                        // FIX: Check if it exists, then update or add
                        if (uriSource.Parameters.Contains(param.Key))
                        {
                            uriSource.Parameters[param.Key].Value = value; // Access the .Value property
                        }
                        else
                        {
                            uriSource.Parameters.Add(param.Key, value); // Use the .Add() method
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to resolve report source: {ex.Message}");
            }

            // 6. Process and Render the Report
            var reportProcessor = new ReportProcessor();
            RenderingResult result;
            try
            {
                result = reportProcessor.RenderReport(format, resolvedReportSource, deviceInfo: null);
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to render report: {ex.Message}");
            }

            if (result.HasErrors)
            {
                return BadRequest("Rendering failed. Check server logs for details.");
            }

            // 7. Return the File
            var fileName = Path.GetFileNameWithoutExtension(safeReportName) + "_" + DateTime.Now.ToString("yyyyMMdd") + "." + result.Extension;
            return File(result.DocumentBytes, contentType, fileName);
        }

        private Dictionary<string, object> GetQueryParametersExcept(params string[] excludedKeys)
        {
            var excluded = new HashSet<string>(excludedKeys ?? Array.Empty<string>(), StringComparer.OrdinalIgnoreCase);

            return Request.Query
                .Where(kvp => !excluded.Contains(kvp.Key))
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => (object)kvp.Value.ToString(),
                    StringComparer.OrdinalIgnoreCase
                );
        }
    }
}