using ApiCamisetas.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace ApiCamisetas.Services
{
    public class ServiceStorageBlobs
    {
        private BlobServiceClient client;
        public ServiceStorageBlobs(BlobServiceClient blobServiceClient)
        {
            this.client=blobServiceClient;
        }

        public async Task<List<string>> GetContainersAsync()
        {
            List<string> containers = new List<string>();
            await foreach (BlobContainerItem item in this.client.GetBlobContainersAsync())
            {
                containers.Add(item.Name);
            }
            return containers;
        }

        public async Task<List<BlobModel>> GetBlobsAsync(string containerName)
        {
            //NECESITAMOS UN CLIENTE DE CONTAINER
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            List<BlobModel> models = new List<BlobModel>();
            await foreach (BlobItem item in containerClient.GetBlobsAsync())
            {
                BlobClient blobClient = containerClient.GetBlobClient(item.Name);
                BlobModel blob = new BlobModel();
                blob.Nombre=item.Name;
                blob.Url=blobClient.Uri.AbsoluteUri;
                blob.Container=containerName;
                models.Add(blob);
            }
            return models;

        }

        //METODO PARA RECUPERAR UN BLOB
        public async Task<BlobModel?> FindBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            if (await blobClient.ExistsAsync())
            {
                BlobModel blobModel = new BlobModel
                {
                    Nombre=blobName,
                    Url=blobClient.Uri.AbsoluteUri,
                    Container=containerName
                };
                return blobModel;
            }
            return null;
        }

        //METODO PARA ELIMINAR UN BLOB
        public async Task DeleteBlobAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);

            await containerClient.DeleteBlobAsync(blobName);
        }

        //METODO PARA SUBIR UN BLOB A UN CONTAINER
        public async Task UploadBlobAsync(string containerName, string blobName, Stream stream)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            await containerClient.UploadBlobAsync(blobName, stream);
        }

        public string GetContainerUrl(string containerName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);

            return containerClient.Uri.AbsoluteUri;
        }
        public async Task<Stream?> GetBlobStreamAsync(string containerName, string blobName)
        {
            BlobContainerClient containerClient = this.client.GetBlobContainerClient(containerName);
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                return response.Value.Content;
            }
            return null;
        }

    }
}
