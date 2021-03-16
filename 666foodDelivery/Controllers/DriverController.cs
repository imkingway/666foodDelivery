using _666foodDelivery.Areas.Identity.Data;
using _666foodDelivery.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace _666foodDelivery.Controllers
{
    public class DriverController : Controller
    {
        private readonly _666foodDeliveryNewContext _context;
        private readonly ILogger<DriverController> _logger;
        private readonly UserManager<_666foodDeliveryUser> _userManger;

        public DriverController(_666foodDeliveryNewContext context, ILogger<DriverController> logger, UserManager<_666foodDeliveryUser> userManager) {
            _context = context;
            _logger = logger;
            _userManger = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Samson() {
            return View();
        }
    }
}
