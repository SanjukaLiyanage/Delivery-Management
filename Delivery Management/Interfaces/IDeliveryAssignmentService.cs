using Delivery_Management.Models;

namespace Delivery_Management.Interfaces
{
    public interface IDeliveryAssignmentService
    {
        Task<List<DeliveryAssignment>> GetAsync();
        Task<DeliveryAssignment?> GetAsync(string id);
        Task<List<DeliveryAssignment>> GetByDriverIdAsync(string driverId);
        Task CreateAsync(DeliveryAssignment newDeliveryAssignment);
        Task UpdateAsync(string id, DeliveryAssignment updatedDeliveryAssignment);
        Task RemoveAsync(string id);
    }
} 