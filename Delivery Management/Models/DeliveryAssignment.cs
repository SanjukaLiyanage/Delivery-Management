using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Delivery_Management.Models
{
    public class DeliveryAssignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("deliveryId")]
        public string DeliveryId { get; set; }

        [BsonElement("driverId")]
        public string DriverId { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } // Pending, InProgress, Completed, Cancelled

        [BsonElement("assignedDate")]
        public DateTime AssignedDate { get; set; }

        [BsonElement("estimatedDeliveryTime")]
        public DateTime EstimatedDeliveryTime { get; set; }

        [BsonElement("notes")]
        public string Notes { get; set; }
    }
} 