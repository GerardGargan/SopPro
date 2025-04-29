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
        /// <summary>
        /// Deletes a file from blob storage if it exists
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <returns>True if it exists and is marked for deletion, false if not</returns>
        public async Task<bool> DeleteBlob(string blobName, string containerName)
        {
            BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            return await blobClient.DeleteIfExistsAsync();
        }

        /// <summary>
        /// Fetches the absolute url for a file in blob storage
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <returns>A string with the url of the specified blob</returns>
        public async Task<string> GetBlob(string blobName, string containerName)
        {
            BlobContainerClient containerClient = _blobClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            return blobClient.Uri.AbsoluteUri;
        }

        /// <summary>
        /// Uploads a blob file to blob storage
        /// </summary>
        /// <param name="blobName"></param>
        /// <param name="containerName"></param>
        /// <param name="file"></param>
        /// <returns>The url of the file if successful, an empty string if unsuccessful</returns>
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