using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using template.Models;

namespace template.Controllers
{
    public class HomeController : Controller
    {
        private MyWebsiteEntities db = new MyWebsiteEntities();

        public ActionResult Index()
        {
            var categories = db.Categories.ToList();

            
            return View(categories);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}