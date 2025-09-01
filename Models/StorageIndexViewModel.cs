using System;
using System.Collections.Generic;

namespace AzureStorageDemo.Models
{
    // View model for the dashboard page.
    public class StorageIndexViewModel
    {
        public List<CustomerEntity> Customers { get; set; } = new();
        public List<ProductEntity> Products { get; set; } = new();

        public List<Uri> BlobUrls { get; set; } = new();
        public List<string> Contracts { get; set; } = new();
    }
}
