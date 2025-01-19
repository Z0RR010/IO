using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using IO.Modules.Communication;

namespace IO.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : ControllerBase
    {
        private readonly RaportManager _raportService;

        public ReportController(RaportManager raportService)
        {
            _raportService = raportService;
        }

        [HttpGet("download")]
        public IActionResult DownloadReport([FromQuery] int daysBefore = 0)
        {
            var reports = GetReportsFromDaysBefore(daysBefore);
            var concatenatedReports = string.Join("\n\n", reports);

            var fileBytes = Encoding.UTF8.GetBytes(concatenatedReports);
            var fileName = $"report_last_{daysBefore}_days.txt";

            return File(fileBytes, "text/plain", fileName);
        }

        private IEnumerable<string> GetReportsFromDaysBefore(int days)
        {
            var reportList = new List<string>();
            
            // Include the active report if it exists
            if (!string.IsNullOrEmpty(_raportService.activeReport))
            {
                reportList.Add(_raportService.activeReport);
            }

            // Add reports from the last 'days' days
            if (_raportService.usedServer?.Reports != null && _raportService.usedServer.Reports.Any())
            {
                int count = _raportService.usedServer.Reports.Count;
                int takeCount = Math.Min(days, count);

                var pastReports = _raportService.usedServer.Reports
                    .Skip(count - takeCount) // Skip the older reports
                    .Take(takeCount)         // Take the most recent reports
                    .Select(report => report);

                reportList.AddRange(pastReports);
            }

            // Return the combined list of reports
            return reportList;
        }
    }
}