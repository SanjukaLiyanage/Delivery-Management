using Delivery_Management.Models;

namespace Delivery_Management.Interfaces
{
    public interface IDriverService
    {
        Task<List<Driver>> GetAsync();
        Task<Driver?> GetAsync(string id);
        Task<Driver?> GetByEmailAsync(string email);
        Task<List<Driver>> GetActiveDriversAsync();
        Task CreateAsync(Driver newDriver);
        Task UpdateAsync(string id, Driver updatedDriver);
        Task RemoveAsync(string id);
    }
} 