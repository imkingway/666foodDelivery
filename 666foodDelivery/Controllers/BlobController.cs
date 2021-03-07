using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Controllers
{
    public class BlobController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot configurationRoot = builder.Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configurationRoot["ConnectionStrings:Bloblstorageconnection"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("images");

            return container;
        }

        public async Task<IActionResult> UploadBlob(IFormFile files)
        {
            CloudBlobContainer cloud = GetCloudBlobContainer();
            CloudBlockBlob blob = cloud.GetBlockBlobReference(files.FileName);
            await blob.UploadFromStreamAsync(files.OpenReadStream());

            var blobUrl = blob.Uri.AbsoluteUri;
            ViewData["BlobUrl"] = blobUrl;

            return View("Create");
        }
    }
}
