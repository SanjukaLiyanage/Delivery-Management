using Microsoft.AspNetCore.Mvc;
using Delivery_Management.Models;
using Delivery_Management.Services;
using Delivery_Management.Interfaces;

namespace Delivery_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryAssignmentController : ControllerBase
    {
        private readonly IDeliveryAssignmentService _deliveryAssignmentService;

        public DeliveryAssignmentController(IDeliveryAssignmentService deliveryAssignmentService) =>
            _deliveryAssignmentService = deliveryAssignmentService;

        [HttpGet]
        public async Task<List<DeliveryAssignment>> Get() =>
            await _deliveryAssignmentService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DeliveryAssignment>> Get(string id)
        {
            var deliveryAssignment = await _deliveryAssignmentService.GetAsync(id);

            if (deliveryAssignment is null)
            {
                return NotFound();
            }

            return deliveryAssignment;
        }

        [HttpGet("driver/{driverId}")]
        public async Task<List<DeliveryAssignment>> GetByDriverId(string driverId) =>
            await _deliveryAssignmentService.GetByDriverIdAsync(driverId);

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] DeliveryAssignment newDeliveryAssignment)
        {
            // Remove the ID if it exists, let MongoDB generate it
            newDeliveryAssignment.Id = null;
            
            await _deliveryAssignmentService.CreateAsync(newDeliveryAssignment);

            return CreatedAtAction(nameof(Get), new { id = newDeliveryAssignment.Id }, newDeliveryAssignment);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, [FromBody] DeliveryAssignment updatedDeliveryAssignment)
        {
            var deliveryAssignment = await _deliveryAssignmentService.GetAsync(id);

            if (deliveryAssignment is null)
            {
                return NotFound();
            }

            updatedDeliveryAssignment.Id = deliveryAssignment.Id;

            await _deliveryAssignmentService.UpdateAsync(id, updatedDeliveryAssignment);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var deliveryAssignment = await _deliveryAssignmentService.GetAsync(id);

            if (deliveryAssignment is null)
            {
                return NotFound();
            }

            await _deliveryAssignmentService.RemoveAsync(id);

            return NoContent();
        }
    }
} 