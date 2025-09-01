using System;

namespace AzureStorageDemo.Models
{
    // A tiny message that we will send to the Azure Queue.
    public class OrderMessage
    {
        public string Type { get; set; } = "";    // e.g., "Processing order"
        public string Detail { get; set; } = "";  // e.g., "ImageName: phone.png"
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}

