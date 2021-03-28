using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FinalProiect.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Acest site este proiectul nostru.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact: ";

            return View();
        }
    }
}