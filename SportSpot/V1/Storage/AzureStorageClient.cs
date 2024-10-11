using Azure.Storage.Blobs;
using Azure.Storage.Sas;

namespace SportSpot.V1.Storage
{
    public class AzureStorageClient(BlobContainerClient _blobContainerClient) : IBlobClient
    {
        public async Task<byte[]> DownloadData(Uri uri)
        {
            BlobClient blobClient = new(uri);
            var result = await blobClient.DownloadContentAsync();
            return result.Value.Content.ToArray();
        }

        public async Task UploadData(string fileName, byte[] data, bool overwrite = false)
        {
            using MemoryStream stream = new(data);
            BlobClient blobClient = _blobContainerClient.GetBlobClient(fileName);
            await blobClient.UploadAsync(stream, overwrite: overwrite);
        }

        public Uri GenerateSaSUri(string fileName, BlobContainerSasPermissions permission)
        {
            BlobSasBuilder sasBuilder = new()
            {
                BlobContainerName = _blobContainerClient.Name,
                BlobName = fileName,
                Resource = "b",
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(1)
            };
            sasBuilder.SetPermissions(permission);
            return _blobContainerClient.GetBlobClient(fileName).GenerateSasUri(sasBuilder);
        }

        public async Task DeleteBlob(string fileName)
        {
            await _blobContainerClient.DeleteBlobAsync(fileName);
        }

        public async Task DeleteBlob(Uri uri)
        {
            BlobClient blobClient = new(uri);
            await blobClient.DeleteAsync();
        }
    }
}
