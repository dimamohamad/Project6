using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using template.Models;

namespace template.Controllers
{
    public class AccountController : Controller
    {
        private MyWebsiteEntities db = new MyWebsiteEntities();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user, string confirmPassword)
        {
            if (ModelState.IsValid)
            {
                if (user.Password != confirmPassword)
                    return View();
                db.Users.Add(user);
                db.SaveChanges();
              
                return RedirectToAction("Login");

            }
            return View(user);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            var user = db.Users.FirstOrDefault(u => u.Email == email && u.Password == password);
            if (user != null)
            {
                Session["Firstname"] = user.FirstName;
                Session["UserId"] = user.UserId;
                return RedirectToAction("Index", "Home");

            }

            ViewBag.ErrorMessage = "Invalid Email or password.";
            return View();
        }

        public ActionResult Logout()
        {
            Session["UserId"] = null;
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Profile()
        {
            int userId = (int)Session["UserId"];
            var user = db.Users.Find(userId);
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Profile(User user, HttpPostedFileBase ImageFile)
        {
            var newUser = db.Users.Find(user.UserId);
            if (ModelState.IsValid)
            {
                // Handle image upload
                if (ImageFile != null && ImageFile.ContentLength > 0)
                {
                    // Generate unique file name
                    string fileName = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                    string extension = Path.GetExtension(ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + extension;

                    // Define file path
                    string filePath = "/Images/" + fileName;

                    // Save file to server
                    string serverPath = Path.Combine(Server.MapPath("~/Images/"), fileName);
                    ImageFile.SaveAs(serverPath);

                    // Update user profile image path
                    newUser.Image = filePath;
                }
                newUser.FirstName = user.FirstName;
                newUser.LastName = user.LastName;
                // Update user details
                db.Entry(newUser).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            return View(newUser);
        }
    }
}
