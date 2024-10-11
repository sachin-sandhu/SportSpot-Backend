using Azure.Storage.Sas;

namespace SportSpot.V1.Storage
{
    public interface IBlobClient
    {
        Task UploadData(string fileName, byte[] data, bool overwrite = false);
        Task<byte[]> DownloadData(Uri uri);
        Uri GenerateSaSUri(string fileName, BlobContainerSasPermissions permission);
        Task DeleteBlob(string fileName);
        Task DeleteBlob(Uri uri);

    }
}
