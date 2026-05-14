using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/part-requests")]
    [ApiController]
    public class PartRequestController(IPartRequestService partRequestService, ILogger<PartRequestController> logger) : ControllerBase
    {
        private readonly IPartRequestService _partRequestService = partRequestService;
        private readonly ILogger<PartRequestController> _logger = logger;

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            _logger.LogInformation("Fetching all part requests");
            var requests = await _partRequestService.GetAllAsync();
            return Ok(requests);
        }

        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetRequestsByCustomer(long customerId)
        {
            _logger.LogInformation("Fetching part requests for customer {CustomerId}", customerId);
            var requests = await _partRequestService.GetByCustomerIdAsync(customerId);
            return Ok(requests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            _logger.LogInformation("Fetching part request {RequestId}", id);
            var request = await _partRequestService.GetByIdAsync(id);

            if (request is null)
                return NotFound(new { message = "Part request not found" });

            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest(PartRequestDTO requestData)
        {
            _logger.LogInformation("Creating part request for customer {CustomerId}", requestData.CustomerId);
            var request = await _partRequestService.AddAsync(requestData);
            return CreatedAtAction(nameof(GetRequestById), new { id = request.Id }, request);
        }

        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteRequest(int id)
        {
            _logger.LogInformation("Completing part request {RequestId}", id);
            var request = await _partRequestService.CompleteAsync(id);

            if (request is null)
                return NotFound(new { message = "Part request not found" });

            return Ok(request);
        }

        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            _logger.LogInformation("Rejecting part request {RequestId}", id);
            var request = await _partRequestService.RejectAsync(id);

            if (request is null)
                return NotFound(new { message = "Part request not found" });

            return Ok(request);
        }

        [HttpDelete("{id:int}/delete")]
        public async Task<IActionResult> DeleteRequest(int id)
        {
            _logger.LogInformation("Deleting part request {RequestId}", id);
            await _partRequestService.DeleteAsync(id);
            return Ok();
        }
    }
}
