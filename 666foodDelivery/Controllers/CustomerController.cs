using _666foodDelivery.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Controllers
{
    public class CustomerController : Controller
    {
        public IActionResult Feedback()
        {
            return View();
        }

        public ActionResult InsertFeedback(string customername, string feedbackcategory, string customerfeedback)
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();
            CloudStorageAccount storageaccount = CloudStorageAccount.Parse(configure["ConnectionStrings:_666foodDeliveryBlobConnection"]);
            CloudTableClient tableClient = storageaccount.CreateCloudTableClient();
            CloudTable table = tableClient.GetTableReference("feedback");

            Feedback feedback = new Feedback("CustomerFeedback", customerfeedback)
            {
                CustomerName = customername,
                FeedbackCategory = feedbackcategory
            };

            try
            {
                TableOperation insert = TableOperation.Insert(feedback);
                TableResult result = table.ExecuteAsync(insert).Result;

                if (result.HttpStatusCode == 204)
                {
                    TempData["InsertResult"] = "Thank you for your feedback!";
                }
            }
            catch (Exception ex)
            {
                TempData["InsertResult"] = "Error occurred, please try again. " + ex.ToString();
            }
            return RedirectToAction("Feedback");
        }
    }
}
