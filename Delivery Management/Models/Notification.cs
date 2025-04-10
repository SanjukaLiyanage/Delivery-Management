using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Delivery_Management.Models
{
    public class Notification
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("userId")]
        public string UserId { get; set; }

        [BsonElement("type")]
        public string Type { get; set; } // Email, SMS

        [BsonElement("title")]
        public string Title { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("isRead")]
        public bool IsRead { get; set; }

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("deliveryId")]
        public string? DeliveryId { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } // Pending, Sent, Failed
    }
} 