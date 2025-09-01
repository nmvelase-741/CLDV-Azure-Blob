using System;
using Azure;
using Azure.Data.Tables;

namespace AzureStorageDemo.Models
{
    public class CustomerEntity : ITableEntity
    {
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        // New property to track creation time
        public DateTime CreatedOn { get; set; }

        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
