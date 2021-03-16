using _666foodDelivery.Models;
using Microsoft.AspNetCore.Http;
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
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Login(string username, string password)
        {
            if (username == "admin" && password == "admin")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Please enter valid username and password";
            }
            return View();
        }

        public ActionResult ViewFeedback()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot configure = builder.Build();
            CloudStorageAccount objectaccount = CloudStorageAccount.Parse(configure["ConnectionStrings:_666foodDeliveryBlobConnection"]);
            CloudTableClient tableclient = objectaccount.CreateCloudTableClient();
            CloudTable table = tableclient.GetTableReference("feedback");

            List<Feedback> feedback = new List<Feedback>();
            try
            {
                TableQuery<Feedback> query = new TableQuery<Feedback>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "CustomerFeedback"));
                TableContinuationToken token = null;

                do
                {
                    TableQuerySegment<Feedback> result = table.ExecuteQuerySegmentedAsync(query, token).Result;
                    token = result.ContinuationToken;

                    foreach (Feedback customer in result.Results)
                    {
                        feedback.Add(customer);
                    }
                } while (token != null);
            }
            catch (Exception ex)
            {
                TempData["msg"] = "Error occurred: " + ex.ToString();
            }

            return View(feedback);
        }
    }
}
