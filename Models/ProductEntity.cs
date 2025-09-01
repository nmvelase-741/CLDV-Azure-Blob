using System;
using Azure;
using Azure.Data.Tables;

namespace AzureStorageDemo.Models
{
    public class ProductEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public DateTime CreatedOn { get; set; } // Timestamp

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
