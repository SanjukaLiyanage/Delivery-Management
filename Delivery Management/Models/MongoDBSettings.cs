namespace Delivery_Management.Models
{
    public class MongoDBSettings
    {
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
        public string DeliveryAssignmentCollectionName { get; set; } = null!;
    }
} 