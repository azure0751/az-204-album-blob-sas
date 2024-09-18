using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace Blob_SAS_album.Controllers
{
    public class HomeController : Controller
    {
        private readonly AzureBlobStorageOptions _blobStorageOptions;

        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;

            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var (images, errorMessage) = await GetImagesAsync();
            ViewBag.ErrorMessage = errorMessage;
            return View(images);
        }

        //private async Task<IEnumerable<string>> GetImagesAsync()
        //{
        //    var images = new List<string>();

        //    string sasToken = _configuration["AzureBlobStorage:SasToken"];
        //    string containerName = _configuration["AzureBlobStorage:ContainerName"];
        //    string storageAccountName = _configuration["AzureBlobStorage:StorageAccount"];

        //    if (string.IsNullOrWhiteSpace(sasToken))
        //    {
        //        throw new InvalidOperationException("SAS Token must be provided.");
        //    }

        //    string blobServiceUri = $"https://{storageAccountName}.blob.core.windows.net";
        //    var blobServiceClient = new BlobServiceClient(new Uri($"{blobServiceUri}?{sasToken}"));

        //    // Get a reference to the container
        //    var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

        //    // List and print all blob URLs in the container
        //    Console.WriteLine($"Listing blobs in container '{containerName}':");

        //    var blobs = containerClient.GetBlobsAsync();
        //    await foreach (var blobItem in containerClient.GetBlobsAsync())
        //    {
        //        var blobClient = containerClient.GetBlobClient(blobItem.Name);
        //        var blobUrl = blobClient.Uri.ToString();
        //        images.Add(blobUrl);
        //        Console.WriteLine(blobUrl); // Print blob URL
        //    }

           
        //    return images;
        //}

        private async Task<(IEnumerable<string>, string)> GetImagesAsync()
        {
            var images = new List<string>();
            string sasToken = _configuration["AzureBlobStorage:SasToken"];
            string containerName = _configuration["AzureBlobStorage:ContainerName"];
            string storageAccountName = _configuration["AzureBlobStorage:StorageAccount"];

            if (string.IsNullOrWhiteSpace(sasToken))
            {
                var errorMessage = "SAS Token must be provided. Please set the environment variable 'AzureBlobStorage__SasToken' or update the 'SasToken' value in the 'appsettings.json'.";
                return (images, errorMessage);
            }

            if (string.IsNullOrWhiteSpace(containerName))
            {
                var errorMessage = "containerName must be provided. Please set the environment variable 'AzureBlobStorage__ContainerName' or update the 'ContainerName' value in the 'appsettings.json'.";
                return (images, errorMessage);
            }

            if (string.IsNullOrWhiteSpace(storageAccountName))
            {
                var errorMessage = "storageAccountName must be provided. Please set the environment variable 'AzureBlobStorage__StorageAccount' or update the 'storageAccountName' value in the 'appsettings.json'.";
                return (images, errorMessage);
            }

            try
            {
                string blobServiceUri = $"https://{storageAccountName}.blob.core.windows.net";
                var blobServiceClient = new BlobServiceClient(new Uri($"{blobServiceUri}?{sasToken}"));
                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

                var blobs = containerClient.GetBlobsAsync();
                 await foreach (var blobItem in containerClient.GetBlobsAsync())
                {
                var blobClient = containerClient.GetBlobClient(blobItem.Name);
                       var blobUrl = blobClient.Uri.ToString();
                       images.Add(blobUrl);
                       Console.WriteLine(blobUrl); // Print blob URL
                 }
            }
            catch (Exception ex)
            {
                var errorMessage = $"Error accessing Blob Storage: {ex.Message}";
                return (images, errorMessage);
            }

            return (images, null);
        }
    }
}