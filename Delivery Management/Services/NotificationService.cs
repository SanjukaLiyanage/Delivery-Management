using Delivery_Management.Models;
using Delivery_Management.Interfaces;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Net.Mail;
using System.Net;
using System.Diagnostics;

namespace Delivery_Management.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMongoCollection<Notification> _notificationCollection;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public NotificationService(IOptions<MongoDBSettings> mongoDBSettings, IConfiguration configuration)
        {
            var mongoClient = new MongoClient(mongoDBSettings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(mongoDBSettings.Value.DatabaseName);
            _notificationCollection = mongoDatabase.GetCollection<Notification>("Notifications");

            // Get SMTP settings from configuration
            _smtpServer = configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = configuration["EmailSettings:Username"] ?? "";
            _smtpPassword = configuration["EmailSettings:Password"] ?? "";
            _fromEmail = configuration["EmailSettings:FromEmail"] ?? "";

            Debug.WriteLine($"SMTP Settings loaded - Server: {_smtpServer}, Port: {_smtpPort}, From: {_fromEmail}");
        }

        public async Task<List<Notification>> GetAsync() =>
            await _notificationCollection.Find(_ => true).ToListAsync();

        public async Task<Notification?> GetAsync(string id) =>
            await _notificationCollection.Find(x => x.Id == id).FirstOrDefaultAsync();

        public async Task<List<Notification>> GetByUserIdAsync(string userId) =>
            await _notificationCollection.Find(x => x.UserId == userId).ToListAsync();

        public async Task CreateAsync(Notification newNotification)
        {
            try
            {
                newNotification.CreatedAt = DateTime.UtcNow;
                newNotification.IsRead = false;
                newNotification.Status = "Pending";

                Debug.WriteLine($"Creating notification for user: {newNotification.UserId}");
                await _notificationCollection.InsertOneAsync(newNotification);
                Debug.WriteLine("Notification saved to database");

                // Send notification based on type
                if (newNotification.Type == "Email")
                {
                    Debug.WriteLine("Attempting to send email notification");
                    await SendEmailNotificationAsync(newNotification);
                    Debug.WriteLine("Email notification sent successfully");
                }
                else if (newNotification.Type == "SMS")
                {
                    await SendSMSNotificationAsync(newNotification);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in CreateAsync: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        public async Task UpdateAsync(string id, Notification updatedNotification)
        {
            await _notificationCollection.ReplaceOneAsync(x => x.Id == id, updatedNotification);
        }

        public async Task MarkAsReadAsync(string id)
        {
            var update = Builders<Notification>.Update.Set(x => x.IsRead, true);
            await _notificationCollection.UpdateOneAsync(x => x.Id == id, update);
        }

        private async Task SendEmailNotificationAsync(Notification notification)
        {
            try
            {
                Debug.WriteLine($"Preparing to send email to: {notification.UserId}");
                using (var client = new SmtpClient(_smtpServer, _smtpPort))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);

                    var message = new MailMessage
                    {
                        From = new MailAddress(_fromEmail),
                        Subject = notification.Title,
                        Body = notification.Message,
                        IsBodyHtml = true
                    };

                    message.To.Add(notification.UserId);
                    Debug.WriteLine($"Email message prepared - From: {_fromEmail}, To: {notification.UserId}");

                    await client.SendMailAsync(message);
                    Debug.WriteLine("Email sent successfully");

                    // Update notification status
                    var update = Builders<Notification>.Update.Set(x => x.Status, "Sent");
                    await _notificationCollection.UpdateOneAsync(x => x.Id == notification.Id, update);
                    Debug.WriteLine("Notification status updated to Sent");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending email: {ex.Message}");
                Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                
                // Update notification status to failed
                var update = Builders<Notification>.Update.Set(x => x.Status, "Failed");
                await _notificationCollection.UpdateOneAsync(x => x.Id == notification.Id, update);
                Debug.WriteLine("Notification status updated to Failed");
                throw;
            }
        }

        private async Task SendSMSNotificationAsync(Notification notification)
        {
            // Implement SMS sending logic here
            // You can use services like Twilio, MessageBird, or any other SMS provider
            // For now, we'll just mark it as sent
            var update = Builders<Notification>.Update.Set(x => x.Status, "Sent");
            await _notificationCollection.UpdateOneAsync(x => x.Id == notification.Id, update);
        }
    }
} 