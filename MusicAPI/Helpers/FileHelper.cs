using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;

namespace MusicAPI.Helpers
{
    public class FileHelper
    {
        public static async Task<string> UploadImage(IFormFile file)
        { 
            string connectionString = @"yourConnectionString";
            string containerName = "songscover";

            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(file.FileName);
            
            var memoryStream = new MemoryStream(); //Stream start
            await file.CopyToAsync(memoryStream); //Recording file to memory stream
            memoryStream.Position = 0; //Stream end

            await blobClient.UploadAsync(memoryStream);
            
            return blobClient.Uri.AbsoluteUri;
        }
        
        public static async Task<string> UploadFile(IFormFile file)
        {
            string connectionString = @"yourConnectionString";
            string containerName = "audiofiles";

            BlobContainerClient blobContainerClient = new BlobContainerClient(connectionString, containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(file.FileName);
            
            var memoryStream = new MemoryStream(); //Stream start
            await file.CopyToAsync(memoryStream); //Recording file to memory stream
            memoryStream.Position = 0; //Stream end

            await blobClient.UploadAsync(memoryStream);
            
            return blobClient.Uri.AbsoluteUri;
        }
    }
}