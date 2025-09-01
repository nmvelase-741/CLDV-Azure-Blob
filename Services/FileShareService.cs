using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace AzureStorageDemo.Services
{
    public class FileShareService
    {
        private readonly ShareClient _share;
        private readonly string _logFileName;

        // Example constructor with 3 arguments
        public FileShareService(string connectionString, string shareName, string logFileName)
        {
            _share = new ShareClient(connectionString, shareName);
            _share.CreateIfNotExists();
            _logFileName = logFileName;
        }

        public async Task UploadContractAsync(Stream content, string fileName)
        {
            var dir = _share.GetRootDirectoryClient();
            var file = dir.GetFileClient(fileName);

            long length = content.CanSeek ? content.Length - content.Position : content.Length;
            await file.CreateAsync(length);
            await file.UploadRangeAsync(new HttpRange(0, length), content);
        }

        public async Task<Stream> DownloadContractAsync(string fileName)
        {
            var dir = _share.GetRootDirectoryClient();
            var file = dir.GetFileClient(fileName);

            if (await file.ExistsAsync())
            {
                ShareFileDownloadInfo download = await file.DownloadAsync();
                return download.Content;
            }

            throw new FileNotFoundException($"Contract '{fileName}' not found.");
        }

        public async Task<List<string>> ListContractsAsync()
        {
            var dir = _share.GetRootDirectoryClient();
            var results = new List<string>();

            await foreach (ShareFileItem item in dir.GetFilesAndDirectoriesAsync())
            {
                if (!item.IsDirectory)
                    results.Add(item.Name);
            }
            return results;
        }

        public async Task WriteLogAsync(string message)
        {
            var dir = _share.GetRootDirectoryClient();
            var logFile = dir.GetFileClient(_logFileName);

            if (!await logFile.ExistsAsync())
            {
                await logFile.CreateAsync(0);
            }

            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(
                $"[{DateTime.UtcNow}] {message}{Environment.NewLine}");
            using var ms = new MemoryStream(bytes);

            var props = await logFile.GetPropertiesAsync();
            long position = props.Value.ContentLength;

            await logFile.UploadRangeAsync(new HttpRange(position, ms.Length), ms);
        }

        internal async Task UploadContractAsync(byte[] bytes, string safeName)
        {
            throw new NotImplementedException();
        }
    }
}
