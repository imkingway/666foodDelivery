using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using _666foodDelivery.Data;
using _666foodDelivery.Models;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Microsoft.AspNetCore.Http;

namespace _666foodDelivery.Views.Foods
{
    public class FoodsController : Controller
    {
        private readonly _666foodDeliveryNewContext _context;

        public FoodsController(_666foodDeliveryNewContext context)
        {
            _context = context;
        }

        // GET: Foods
        public async Task<IActionResult> Index()
        {
            return View(await _context.Food.ToListAsync());
        }

        // GET: Foods/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Food
                .FirstOrDefaultAsync(m => m.ID == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        // GET: Foods/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Foods/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,FoodName,FoodProducedDate,Type,Price,Ingredient,BlobURL")] Food food)
        {
            if (ModelState.IsValid)
            {
                _context.Add(food);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(food);
        }

        // GET: Foods/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Food.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }
            return View(food);
        }

        // POST: Foods/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FoodName,FoodProducedDate,Type,Price,Ingredient,BlobURL")] Food food)
        {
            if (id != food.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //delete old image
                    var foods = await _context.Food.AsNoTracking().FirstOrDefaultAsync(m => m.ID == id);
                    CloudBlobContainer container = GetCloudBlobContainer();
                    string blobURL = foods.BlobURL;
                    var temp = new Uri(blobURL).LocalPath;
                    string filename = temp.Remove(0, temp.IndexOf('/', 1) + 1);
                    CloudBlockBlob blobs = container.GetBlockBlobReference(filename);
                    await blobs.DeleteIfExistsAsync();
                    
                    _context.Update(food);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodExists(food.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(food);
        }

        // GET: Foods/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var food = await _context.Food
                .FirstOrDefaultAsync(m => m.ID == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }

        // POST: Foods/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var food = await _context.Food.FindAsync(id);

            CloudBlobContainer container = GetCloudBlobContainer();
            string blobURL = food.BlobURL;
            var temp = new Uri(blobURL).LocalPath;
            string filename = temp.Remove(0, temp.IndexOf('/', 1) + 1);
            CloudBlockBlob blob = container.GetBlockBlobReference(filename);
            await blob.DeleteIfExistsAsync();

            _context.Food.Remove(food);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FoodExists(int id)
        {
            return _context.Food.Any(e => e.ID == id);
        }

        private CloudBlobContainer GetCloudBlobContainer()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            IConfigurationRoot configurationRoot = builder.Build();
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(configurationRoot["ConnectionStrings:_666foodDeliveryBlobConnection"]);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("images");

            return container;
        }

        public async Task<IActionResult> UploadBlob(IFormFile files)
        {
            CloudBlobContainer cloud = GetCloudBlobContainer();
            if (files != null)
            {
                if (Path.GetExtension(files.FileName).Equals(".jpg") || Path.GetExtension(files.FileName).Equals(".png"))
                {
                    CloudBlockBlob blob = cloud.GetBlockBlobReference(files.FileName);
                    await blob.UploadFromStreamAsync(files.OpenReadStream());
                    var blobUrl = blob.Uri.AbsoluteUri;
                    ViewData["BlobUrl"] = blobUrl;
                }
                else
                {
                    TempData["Files"] = "Please choose an image.";
                }
            }
            else
            {
                TempData["Files"] = "Please choose an image.";
            }
            return View("Create");
        }

        public async Task<IActionResult> EditImage(IFormFile files, Food food)
        {
            CloudBlobContainer cloud = GetCloudBlobContainer();

            if (files != null)
            {
                if (Path.GetExtension(files.FileName).Equals(".jpg") || Path.GetExtension(files.FileName).Equals(".png"))
                {
                    CloudBlockBlob blob = cloud.GetBlockBlobReference(files.FileName);

                    //upload new image
                    await blob.UploadFromStreamAsync(files.OpenReadStream());
                    var blobUrl = blob.Uri.AbsoluteUri;
                    ViewData["BlobUrl"] = blobUrl;
                }
                else
                {
                    TempData["Files"] = "Please choose an image.";
                }
            }
            else
            {
                TempData["Files"] = "Please choose an image.";
            }
            int id = int.Parse(Request.Form["imageProductId"]);
            var foods = await _context.Food.FirstOrDefaultAsync(m => m.ID == id);
            food.ID = foods.ID;
            food.FoodName = foods.FoodName;
            food.FoodProducedDate = foods.FoodProducedDate;
            food.Type = foods.Type;
            food.Price = foods.Price;
            food.Ingredient = foods.Ingredient;

            return View("Edit", food);
        }
    }
}
