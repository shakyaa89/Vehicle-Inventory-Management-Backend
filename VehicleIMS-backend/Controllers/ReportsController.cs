using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportService _reportService;
        private readonly IPDFService _pdfService;

        public ReportsController(
            IReportService reportService,
            IPDFService pdfService)
        {
            _reportService = reportService;
            _pdfService = pdfService;
        }

        [HttpGet("financial/pdf")]
        public async Task<IActionResult> GetFinancialReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            var report = await _reportService.GenerateAsync(from, to);

            var pdf = _pdfService.GenerateFinancialReport(report);

            return File(pdf, "application/pdf", "financial-report.pdf");
        }
    }
}