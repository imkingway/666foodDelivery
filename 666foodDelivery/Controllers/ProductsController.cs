using _666foodDelivery.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Views.Products
{
    public class ProductsController : Controller
    {
        private readonly _666foodDeliveryNewContext _context;

        public ProductsController(_666foodDeliveryNewContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Food.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound("no id found");
            }

            var food = await _context.Food.FirstOrDefaultAsync(m => m.ID == id);
            if (food == null)
            {
                return NotFound();
            }

            return View(food);
        }
    }
}
