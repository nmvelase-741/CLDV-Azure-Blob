using Azure.Storage.Queues;
using AzureStorageDemo.Models;
using System.Text.Json;
using System.Threading.Tasks;

namespace AzureStorageDemo.Services
{
    // Sends small JSON messages to an Azure Queue.
    public class QueueStorageService
    {
        private readonly QueueClient _queue;

        public QueueStorageService(string connectionString, string queueName)
        {
            _queue = new QueueClient(connectionString, queueName);
            _queue.CreateIfNotExists();
        }

        public async Task SendAsync(OrderMessage message)
        {
            var json = JsonSerializer.Serialize(message);
            await _queue.SendMessageAsync(json);
        }
    }
}

