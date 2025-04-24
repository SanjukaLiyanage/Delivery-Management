using Microsoft.AspNetCore.Mvc;
using Delivery_Management.Models;
using Delivery_Management.Services;
using Delivery_Management.Interfaces;
using System.Text.Json;

namespace Delivery_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DeliveryAssignmentController : ControllerBase
    {
        private readonly IDeliveryAssignmentService _deliveryAssignmentService;
        private readonly IDriverService _driverService;

        public DeliveryAssignmentController(
            IDeliveryAssignmentService deliveryAssignmentService,
            IDriverService driverService)
        {
            _deliveryAssignmentService = deliveryAssignmentService;
            _driverService = driverService;
        }

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
            // Log the received data
            Console.WriteLine($"Received delivery assignment: {JsonSerializer.Serialize(newDeliveryAssignment)}");

            // Validate OrderIds
            if (newDeliveryAssignment.OrderIds == null || !newDeliveryAssignment.OrderIds.Any())
            {
                return BadRequest("At least one order ID must be provided");
            }

            // Validate driver exists
            var driver = await _driverService.GetAsync(newDeliveryAssignment.DriverId);
            if (driver is null)
            {
                return BadRequest("Driver not found");
            }

            // Initialize OrderIds if null
            newDeliveryAssignment.OrderIds ??= new List<string>();

            // Set default values
            newDeliveryAssignment.Status = "Pending";
            newDeliveryAssignment.AssignedDate = DateTime.UtcNow;

            // Log the data being sent to service
            Console.WriteLine($"Sending to service: {JsonSerializer.Serialize(newDeliveryAssignment)}");
            
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

            // Validate OrderIds
            if (updatedDeliveryAssignment.OrderIds == null || !updatedDeliveryAssignment.OrderIds.Any())
            {
                return BadRequest("At least one order ID must be provided");
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