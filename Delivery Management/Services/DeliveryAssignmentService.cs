using Delivery_Management.Models;
using Delivery_Management.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Diagnostics;

namespace Delivery_Management.Services
{
    public class DeliveryAssignmentService : IDeliveryAssignmentService
    {
        private readonly IMongoCollection<DeliveryAssignment> _deliveryAssignmentCollection;
        private readonly INotificationService _notificationService;
        private readonly IDriverService _driverService;

        public DeliveryAssignmentService(
            IOptions<MongoDBSettings> mongoDBSettings, 
            INotificationService notificationService,
            IDriverService driverService)
        {
            var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _deliveryAssignmentCollection = mongoDatabase.GetCollection<DeliveryAssignment>(mongoDBSettings.Value.DeliveryAssignmentCollectionName);
            _notificationService = notificationService;
            _driverService = driverService;
        }

        public async Task<List<DeliveryAssignment>> GetAsync() =>
            await _deliveryAssignmentCollection.Find(_ => true).ToListAsync();

        public async Task<DeliveryAssignment?> GetAsync(string id) =>
            await _deliveryAssignmentCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task CreateAsync(DeliveryAssignment newDeliveryAssignment)
        {
            try
            {
                // Get driver's email
                var driver = await _driverService.GetAsync(newDeliveryAssignment.DriverId);
                if (driver == null)
                {
                    throw new Exception($"Driver with ID {newDeliveryAssignment.DriverId} not found");
                }

                Debug.WriteLine($"Found driver: {driver.Name} with email: {driver.Email}");

                await _deliveryAssignmentCollection.InsertOneAsync(newDeliveryAssignment);

                // Send notification to driver
                var notification = new Notification
                {
                    UserId = driver.Email, // Use driver's email
                    Type = "Email",
                    Title = "New Delivery Assignment",
                    Message = $"Dear {driver.Name},\n\nYou have been assigned a new delivery.\n\nDelivery ID: {newDeliveryAssignment.DeliveryId}\nStatus: {newDeliveryAssignment.Status}\nAssigned Date: {newDeliveryAssignment.AssignedDate}\nEstimated Delivery Time: {newDeliveryAssignment.EstimatedDeliveryTime}\n\nBest regards,\nDelivery Management Team",
                    DeliveryId = newDeliveryAssignment.DeliveryId
                };

                Debug.WriteLine($"Sending notification to: {notification.UserId}");
                await _notificationService.CreateAsync(notification);
                Debug.WriteLine("Notification sent successfully");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task UpdateAsync(string id, DeliveryAssignment updatedDeliveryAssignment)
        {
            var existingAssignment = await GetAsync(id);
            if (existingAssignment != null)
            {
                // Get driver's email
                var driver = await _driverService.GetAsync(updatedDeliveryAssignment.DriverId);
                if (driver == null)
                {
                    throw new Exception($"Driver with ID {updatedDeliveryAssignment.DriverId} not found");
                }

                Debug.WriteLine($"Found driver for update: {driver.Name} with email: {driver.Email}");

                // Check if status has changed
                if (existingAssignment.Status != updatedDeliveryAssignment.Status)
                {
                    // Send notification about status change
                    var notification = new Notification
                    {
                        UserId = driver.Email, // Use driver's email
                        Type = "Email",
                        Title = "Delivery Status Updated",
                        Message = $"Dear {driver.Name},\n\nThe status of your delivery has been updated.\n\nDelivery ID: {updatedDeliveryAssignment.DeliveryId}\nNew Status: {updatedDeliveryAssignment.Status}\n\nBest regards,\nDelivery Management Team",
                        DeliveryId = updatedDeliveryAssignment.DeliveryId
                    };

                    Debug.WriteLine($"Sending status update notification to: {notification.UserId}");
                    await _notificationService.CreateAsync(notification);
                    Debug.WriteLine("Status update notification sent successfully");
                }
            }

            await _deliveryAssignmentCollection.ReplaceOneAsync(x => x.Id == id, updatedDeliveryAssignment);
        }

        public async Task RemoveAsync(string id)
        {
            var assignment = await GetAsync(id);
            if (assignment != null)
            {
                // Get driver's email
                var driver = await _driverService.GetAsync(assignment.DriverId);
                if (driver != null)
                {
                    Debug.WriteLine($"Found driver for removal: {driver.Name} with email: {driver.Email}");

                    // Send notification about cancellation
                    var notification = new Notification
                    {
                        UserId = driver.Email, // Use driver's email
                        Type = "Email",
                        Title = "Delivery Assignment Cancelled",
                        Message = $"Dear {driver.Name},\n\nYour delivery assignment has been cancelled.\n\nDelivery ID: {assignment.DeliveryId}\n\nBest regards,\nDelivery Management Team",
                        DeliveryId = assignment.DeliveryId
                    };

                    Debug.WriteLine($"Sending cancellation notification to: {notification.UserId}");
                    await _notificationService.CreateAsync(notification);
                    Debug.WriteLine("Cancellation notification sent successfully");
                }
            }

            await _deliveryAssignmentCollection.DeleteOneAsync(x => x.Id == id);
        }

        public async Task<List<DeliveryAssignment>> GetByDriverIdAsync(string driverId) =>
            await _deliveryAssignmentCollection.Find(x => x.DriverId == driverId).ToListAsync();
    }
} 