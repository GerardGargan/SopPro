using System.Reflection.Metadata;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Backend.Service.Interface;

namespace Backend.Service.Implementation
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobClient;
        public BlobService(BlobServiceClient blobClient)
        {
            _blobClient = blobClient;
        }
        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<string> GetBlob(string blobName, string containerName)
        {
            BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<string> UploadBlob(string blobName, string containerName, IFormFile file)
        {
            BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            var httpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };
            var result = await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
            if (result != null)
            {
                return await GetBlob(blobName, containerName);
            }

            return "";
        }
    }
}