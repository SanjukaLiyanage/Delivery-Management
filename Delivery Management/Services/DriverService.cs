using Delivery_Management.Models;
using Delivery_Management.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Delivery_Management.Services
{
    public class DriverService : IDriverService
    {
        private readonly IMongoCollection<Driver> _driverCollection;
        private readonly INotificationService _notificationService;

        public DriverService(IOptions<MongoDBSettings> mongoDBSettings, INotificationService notificationService)
        {
            var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _driverCollection = mongoDatabase.GetCollection<Driver>("Drivers");
            _notificationService = notificationService;
        }

        public async Task<List<Driver>> GetAsync() =>
            await _driverCollection.Find(_ => true).ToListAsync();

        public async Task<Driver?> GetAsync(string id) =>
            await _driverCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<Driver?> GetByEmailAsync(string email) =>
            await _driverCollection.Find(x => x.Email == email).FirstOrDefaultAsync();

        public async Task CreateAsync(Driver newDriver)
        {
            newDriver.CreatedAt = DateTime.UtcNow;
            newDriver.LastUpdated = DateTime.UtcNow;
            newDriver.Status = "Active";

            await _driverCollection.InsertOneAsync(newDriver);

            // Send welcome notification
            var notification = new Notification
            {
                UserId = newDriver.Email,
                Type = "Email",
                Title = "Welcome to Delivery Management System",
                Message = $"Dear {newDriver.Name},\n\nWelcome to our delivery management system. Your account has been created successfully.\n\nYour driver ID: {newDriver.Id}\nEmail: {newDriver.Email}\n\nBest regards,\nDelivery Management Team",
                Status = "Pending"
            };

            await _notificationService.CreateAsync(notification);
        }

        public async Task UpdateAsync(string id, Driver updatedDriver)
        {
            var existingDriver = await GetAsync(id);
            if (existingDriver != null)
            {
                updatedDriver.Id = existingDriver.Id;
                updatedDriver.CreatedAt = existingDriver.CreatedAt;
                updatedDriver.LastUpdated = DateTime.UtcNow;

                // If email is changed, send notification
                if (existingDriver.Email != updatedDriver.Email)
                {
                    var notification = new Notification
                    {
                        UserId = existingDriver.Email,
                        Type = "Email",
                        Title = "Email Address Updated",
                        Message = $"Dear {updatedDriver.Name},\n\nYour email address has been updated in our system.\n\nNew email: {updatedDriver.Email}\n\nBest regards,\nDelivery Management Team",
                        Status = "Pending"
                    };

                    await _notificationService.CreateAsync(notification);
                }

                // If status is changed, send notification
                if (existingDriver.Status != updatedDriver.Status)
                {
                    var notification = new Notification
                    {
                        UserId = updatedDriver.Email,
                        Type = "Email",
                        Title = "Driver Status Updated",
                        Message = $"Dear {updatedDriver.Name},\n\nYour status has been updated to: {updatedDriver.Status}\n\nBest regards,\nDelivery Management Team",
                        Status = "Pending"
                    };

                    await _notificationService.CreateAsync(notification);
                }
            }

            await _driverCollection.ReplaceOneAsync(x => x.Id == id, updatedDriver);
        }

        public async Task RemoveAsync(string id)
        {
            var driver = await GetAsync(id);
            if (driver != null)
            {
                // Send notification about account deactivation
                var notification = new Notification
                {
                    UserId = driver.Email,
                    Type = "Email",
                    Title = "Account Deactivated",
                    Message = $"Dear {driver.Name},\n\nYour account has been deactivated from our delivery management system.\n\nBest regards,\nDelivery Management Team",
                    Status = "Pending"
                };

                await _notificationService.CreateAsync(notification);
            }

            await _driverCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<Driver>> GetActiveDriversAsync() =>
            await _driverCollection.Find(x => x.Status == "Active").ToListAsync();
    }
} 