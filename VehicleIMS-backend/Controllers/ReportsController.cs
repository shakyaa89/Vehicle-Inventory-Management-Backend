using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [ApiController]
    [Route("api/reports")]
    // Controller for report generation endpoints
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

        // Generate a financial report as a PDF
        [HttpGet("financial/pdf")]
        public async Task<IActionResult> GetFinancialReport(
            [FromQuery] DateTime from,
            [FromQuery] DateTime to)
        {
            // Build report data and convert to PDF
            var report = await _reportService.GenerateAsync(from, to);

            var pdf = _pdfService.GenerateFinancialReport(report);

            return File(pdf, "application/pdf", "financial-report.pdf");
        }

        // Get regular customers within a date range
        [HttpGet("customers/regulars")]
        public async Task<IActionResult> GetRegularCustomers([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetRegularCustomersAsync(from, to, top);
            return Ok(list);
        }

        // Generate regular customers report as a PDF
        [HttpGet("customers/regulars/pdf")]
        public async Task<IActionResult> GetRegularCustomersPdf([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetRegularCustomersAsync(from, to, top);
            var pdf = _pdfService.GenerateRegularCustomersReport(list, from, to);
            var fileName = $"regular-customers-{from:yyyy-MM-dd}-{to:yyyy-MM-dd}.pdf";
            return File(pdf, "application/pdf", fileName);
        }

        // Get high spenders within a date range
        [HttpGet("customers/high-spenders")]
        public async Task<IActionResult> GetHighSpenders([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetHighSpendersAsync(from, to, top);
            return Ok(list);
        }

        // Generate high spenders report as a PDF
        [HttpGet("customers/high-spenders/pdf")]
        public async Task<IActionResult> GetHighSpendersPdf([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetHighSpendersAsync(from, to, top);
            var pdf = _pdfService.GenerateHighSpendersReport(list, from, to);
            var fileName = $"high-spenders-{from:yyyy-MM-dd}-{to:yyyy-MM-dd}.pdf";
            return File(pdf, "application/pdf", fileName);
        }

        // Get customers with pending credits
        [HttpGet("customers/pending-credits")]
        public async Task<IActionResult> GetPendingCredits([FromQuery] int olderThanDays = 30)
        {
            var list = await _reportService.GetPendingCreditsAsync(olderThanDays);
            return Ok(list);
        }

        // Generate pending credits report as a PDF
        [HttpGet("customers/pending-credits/pdf")]
        public async Task<IActionResult> GetPendingCreditsPdf([FromQuery] int olderThanDays = 30)
        {
            // Clamp to non-negative days to avoid invalid ranges
            var safeDays = Math.Max(0, olderThanDays);
            var list = await _reportService.GetPendingCreditsAsync(safeDays);
            var pdf = _pdfService.GeneratePendingCreditsReport(list, safeDays);
            var fileName = $"pending-credits-{safeDays}-days.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}