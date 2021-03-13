using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
            return View();
        }
    }
}
