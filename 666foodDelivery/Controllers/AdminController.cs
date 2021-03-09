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

        public IActionResult Login(IFormCollection formCollection)
        {
            string email = Convert.ToString(formCollection["username"]);
            string Password = Convert.ToString(formCollection["password"]);
            if (email == "admin" && Password == "admin")
            {
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Message = "Please enter valid email and password";
            }
            return View();
        }

        public ActionResult ViewFeedback()
        {
            return View();
        }
    }
}
