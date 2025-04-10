using Microsoft.AspNetCore.Mvc;
using Delivery_Management.Models;
using Delivery_Management.Interfaces;

namespace Delivery_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;

        public DriverController(IDriverService driverService) =>
            _driverService = driverService;

        [HttpGet]
        public async Task<List<Driver>> Get() =>
            await _driverService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Driver>> Get(string id)
        {
            var driver = await _driverService.GetAsync(id);

            if (driver is null)
            {
                return NotFound();
            }

            return driver;
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<Driver>> GetByEmail(string email)
        {
            var driver = await _driverService.GetByEmailAsync(email);

            if (driver is null)
            {
                return NotFound();
            }

            return driver;
        }

        [HttpGet("active")]
        public async Task<List<Driver>> GetActiveDrivers() =>
            await _driverService.GetActiveDriversAsync();

        [HttpPost]
        public async Task<IActionResult> Post(Driver newDriver)
        {
            await _driverService.CreateAsync(newDriver);

            return CreatedAtAction(nameof(Get), new { id = newDriver.Id }, newDriver);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Driver updatedDriver)
        {
            var driver = await _driverService.GetAsync(id);

            if (driver is null)
            {
                return NotFound();
            }

            updatedDriver.Id = driver.Id;

            await _driverService.UpdateAsync(id, updatedDriver);

            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public async Task<IActionResult> Delete(string id)
        {
            var driver = await _driverService.GetAsync(id);

            if (driver is null)
            {
                return NotFound();
            }

            await _driverService.RemoveAsync(id);

            return NoContent();
        }
    }
} 