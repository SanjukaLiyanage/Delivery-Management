using Microsoft.AspNetCore.Mvc;
using Delivery_Management.Models;
using Delivery_Management.Interfaces;

namespace Delivery_Management.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService) =>
            _notificationService = notificationService;

        [HttpGet]
        public async Task<List<Notification>> Get() =>
            await _notificationService.GetAsync();

        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<Notification>> Get(string id)
        {
            var notification = await _notificationService.GetAsync(id);

            if (notification is null)
            {
                return NotFound();
            }

            return notification;
        }

        [HttpGet("user/{userId}")]
        public async Task<List<Notification>> GetByUserId(string userId) =>
            await _notificationService.GetByUserIdAsync(userId);

        [HttpPost]
        public async Task<IActionResult> Post(Notification newNotification)
        {
            await _notificationService.CreateAsync(newNotification);

            return CreatedAtAction(nameof(Get), new { id = newNotification.Id }, newNotification);
        }

        [HttpPut("{id:length(24)}")]
        public async Task<IActionResult> Update(string id, Notification updatedNotification)
        {
            var notification = await _notificationService.GetAsync(id);

            if (notification is null)
            {
                return NotFound();
            }

            updatedNotification.Id = notification.Id;

            await _notificationService.UpdateAsync(id, updatedNotification);

            return NoContent();
        }

        [HttpPut("{id:length(24)}/read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            var notification = await _notificationService.GetAsync(id);

            if (notification is null)
            {
                return NotFound();
            }

            await _notificationService.MarkAsReadAsync(id);

            return NoContent();
        }
    }
} 