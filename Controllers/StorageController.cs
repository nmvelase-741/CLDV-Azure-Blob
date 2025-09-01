using System;
using System.IO;
using System.Threading.Tasks;
using AzureStorageDemo.Models;
using AzureStorageDemo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AzureStorageDemo.Controllers
{
    public class StorageController : Controller
    {
        private readonly TableStorageService _tables;
        private readonly BlobStorageService _blobs;
        private readonly QueueStorageService _queue;
        private readonly FileShareService _files;

        public StorageController(
            TableStorageService tables,
            BlobStorageService blobs,
            QueueStorageService queue,
            FileShareService files)
        {
            _tables = tables;
            _blobs = blobs;
            _queue = queue;
            _files = files;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new StorageIndexViewModel
            {
                Customers = await _tables.GetCustomersAsync(),
                Products = await _tables.GetProductsAsync(),
                BlobUrls = await _blobs.ListAsync(),
                Contracts = await _files.ListContractsAsync()
            };
            return View(vm);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> AddCustomer(string firstName, string lastName, string email)
        {
            // Create and add a new customer with CreatedOn timestamp
            await _tables.AddCustomerAsync(new CustomerEntity
            {
                PartitionKey = "Customer",          // Azure Table partition
                RowKey = Guid.NewGuid().ToString(), // Unique row key
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                CreatedOn = DateTime.UtcNow         // Timestamp for creation
            });

            await _files.WriteLogAsync($"Added customer {firstName} {lastName}");
            return RedirectToAction(nameof(Index));
        }


        [HttpPost]
        public async Task<IActionResult> AddProduct(string name, string description, decimal price)
        {
            await _tables.AddProductAsync(new ProductEntity
            {
                Name = name,
                Description = description,
                Price = (double)price
            });

            await _files.WriteLogAsync($"Added product {name} @ {price}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> UploadMedia(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var safeName = Path.GetFileName(file.FileName);
                using var stream = file.OpenReadStream();
                await _blobs.UploadAsync(stream, safeName);
                await _queue.SendAsync(new OrderMessage
                {
                    Type = "Upload",
                    Detail = $"ImageName: {safeName}"
                });
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> SendQueueMessage(string type, string detail)
        {
            await _queue.SendAsync(new OrderMessage { Type = type, Detail = detail });
            await _files.WriteLogAsync($"Queue message sent: {type} - {detail}");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UploadContract(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var safeName = Path.GetFileName(file.FileName);
                using var ms = new MemoryStream();
                await file.CopyToAsync(ms);
                var bytes = ms.ToArray();

                await _files.UploadContractAsync(bytes, safeName);  // <-- pass bytes
                await _files.WriteLogAsync($"Contract uploaded: {safeName}");
            }
            return RedirectToAction(nameof(Index));
        }


        public async Task<IActionResult> DownloadContract(string name)
        {
            var fileStream = await _files.DownloadContractAsync(name);
            return File(fileStream, "application/octet-stream", name);
        }
    }
}