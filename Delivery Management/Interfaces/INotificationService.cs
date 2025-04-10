using Delivery_Management.Models;

namespace Delivery_Management.Interfaces
{
    public interface INotificationService
    {
        Task<List<Notification>> GetAsync();
        Task<Notification?> GetAsync(string id);
        Task<List<Notification>> GetByUserIdAsync(string userId);
        Task CreateAsync(Notification newNotification);
        Task UpdateAsync(string id, Notification updatedNotification);
        Task MarkAsReadAsync(string id);
    }
} 