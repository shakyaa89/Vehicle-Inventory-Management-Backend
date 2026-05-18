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

        [HttpGet("customers/regulars")]
        public async Task<IActionResult> GetRegularCustomers([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetRegularCustomersAsync(from, to, top);
            return Ok(list);
        }

        [HttpGet("customers/regulars/pdf")]
        public async Task<IActionResult> GetRegularCustomersPdf([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetRegularCustomersAsync(from, to, top);
            var pdf = _pdfService.GenerateRegularCustomersReport(list, from, to);
            var fileName = $"regular-customers-{from:yyyy-MM-dd}-{to:yyyy-MM-dd}.pdf";
            return File(pdf, "application/pdf", fileName);
        }

        [HttpGet("customers/high-spenders")]
        public async Task<IActionResult> GetHighSpenders([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetHighSpendersAsync(from, to, top);
            return Ok(list);
        }

        [HttpGet("customers/high-spenders/pdf")]
        public async Task<IActionResult> GetHighSpendersPdf([FromQuery] DateTime from, [FromQuery] DateTime to, [FromQuery] int top = 50)
        {
            var list = await _reportService.GetHighSpendersAsync(from, to, top);
            var pdf = _pdfService.GenerateHighSpendersReport(list, from, to);
            var fileName = $"high-spenders-{from:yyyy-MM-dd}-{to:yyyy-MM-dd}.pdf";
            return File(pdf, "application/pdf", fileName);
        }

        [HttpGet("customers/pending-credits")]
        public async Task<IActionResult> GetPendingCredits([FromQuery] int olderThanDays = 30)
        {
            var list = await _reportService.GetPendingCreditsAsync(olderThanDays);
            return Ok(list);
        }

        [HttpGet("customers/pending-credits/pdf")]
        public async Task<IActionResult> GetPendingCreditsPdf([FromQuery] int olderThanDays = 30)
        {
            var safeDays = Math.Max(0, olderThanDays);
            var list = await _reportService.GetPendingCreditsAsync(safeDays);
            var pdf = _pdfService.GeneratePendingCreditsReport(list, safeDays);
            var fileName = $"pending-credits-{safeDays}-days.pdf";
            return File(pdf, "application/pdf", fileName);
        }
    }
}