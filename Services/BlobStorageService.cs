using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace AzureStorageDemo.Services
{
    // Handles image/media uploads and listing via Azure Blob Storage.
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(string connectionString, string containerName)
        {
            _container = new BlobContainerClient(connectionString, containerName);
            _container.CreateIfNotExists(PublicAccessType.Blob);
        }

        public async Task<Uri> UploadAsync(Stream stream, string fileName)
        {
            var blob = _container.GetBlobClient(fileName);
            await blob.UploadAsync(stream, overwrite: true);
            return blob.Uri;
        }

        public async Task<List<Uri>> ListAsync()
        {
            var list = new List<Uri>();
            await foreach (var item in _container.GetBlobsAsync())
            {
                var blobClient = _container.GetBlobClient(item.Name);
                list.Add(blobClient.Uri);
            }
            return list;
        }
    }
}

