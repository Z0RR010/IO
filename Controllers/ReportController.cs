using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using IO.Modules.Communication;
namespace Io.Controllers
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
        public IActionResult DownloadReport()
        {
            var reportContent = _raportService.activeReport ?? "No report available.";
            var fileBytes = Encoding.UTF8.GetBytes(reportContent);
            var fileName = "report.txt";
            
            return File(fileBytes, "text/plain", fileName);
        }
    }
}