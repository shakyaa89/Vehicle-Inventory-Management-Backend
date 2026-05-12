using Microsoft.AspNetCore.Mvc;
using VehicleIMS_backend.Application.DTO;
using VehicleIMS_backend.Application.Interfaces.IServices;

namespace VehicleIMS_backend.Controllers
{
    [Route("api/part-requests")]
    [ApiController]
    public class PartRequestController(IPartRequestService partRequestService) : ControllerBase
    {
        private readonly IPartRequestService _partRequestService = partRequestService;

        [HttpGet]
        public async Task<IActionResult> GetAllRequests()
        {
            var requests = await _partRequestService.GetAllAsync();
            return Ok(requests);
        }

        [HttpGet("customer/{customerId:long}")]
        public async Task<IActionResult> GetRequestsByCustomer(long customerId)
        {
            var requests = await _partRequestService.GetByCustomerIdAsync(customerId);
            return Ok(requests);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetRequestById(int id)
        {
            var request = await _partRequestService.GetByIdAsync(id);

            if (request is null)
                return NotFound(new { message = "Part request not found" });

            return Ok(request);
        }

        [HttpPost]
        public async Task<IActionResult> AddRequest(PartRequestDTO requestData)
        {
            var request = await _partRequestService.AddAsync(requestData);
            return CreatedAtAction(nameof(GetRequestById), new { id = request.Id }, request);
        }

        [HttpPut("{id:int}/complete")]
        public async Task<IActionResult> CompleteRequest(int id)
        {
            var request = await _partRequestService.CompleteAsync(id);

            if (request is null)
                return NotFound(new { message = "Part request not found" });

            return Ok(request);
        }

        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> RejectRequest(int id)
        {
            var request = await _partRequestService.RejectAsync(id);

            if (request is null)
                return NotFound(new { message = "Part request not found" });

            return Ok(request);
        }
    }
}
