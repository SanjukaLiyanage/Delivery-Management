using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Delivery_Management.Models
{
    [BsonIgnoreExtraElements]
    public class DeliveryAssignment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("deliveryId")]
        public string DeliveryId { get; set; } = null!;

        [BsonElement("driverId")]
        public string DriverId { get; set; } = null!;

        [BsonElement("orderIds")]
        [BsonIgnoreIfNull]
        public List<string> OrderIds { get; set; } = new List<string>();

        [BsonElement("status")]
        public string Status { get; set; } = null!;

        [BsonElement("assignedDate")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime AssignedDate { get; set; }

        [BsonElement("estimatedDeliveryTime")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime EstimatedDeliveryTime { get; set; }

        [BsonElement("notes")]
        public string? Notes { get; set; }
    }
} 