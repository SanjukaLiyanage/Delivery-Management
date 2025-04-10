using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Delivery_Management.Models
{
    public class Driver
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("phoneNumber")]
        public string PhoneNumber { get; set; }

        [BsonElement("licenseNumber")]
        public string LicenseNumber { get; set; }

        [BsonElement("status")]
        public string Status { get; set; } = "Active"; // Set default value to "Active"

        [BsonElement("createdAt")]
        public DateTime CreatedAt { get; set; }

        [BsonElement("lastUpdated")]
        public DateTime LastUpdated { get; set; }
    }
} 