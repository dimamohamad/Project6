//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;
//using template.Models;

//namespace template.Controllers
//{
//    public class ProductController : Controller
//    {

//        private MyWebsiteEntities db = new MyWebsiteEntities();

//        public ActionResult Shop(int? categoryId)
//        {
//            var products = categoryId == null ?
//                db.Products.ToList() :
//                db.Products.Where(p => p.CategoryId == categoryId).ToList();
//            return View(products);
//        }

//        public ActionResult Details(int productId)
//        {
//            var product = db.Products.Find(productId);
//            if (product == null)
//            {
//                return HttpNotFound();
//            }
//            return View(product);
//        }

//        public ActionResult AddToCart(int productId)
//        {
//            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
//            var item = cart.FirstOrDefault(c => c.ProductId == productId);

//            if (item != null)
//            {
//                item.Quantity++;
//            }
//            else
//            {
//                var product = db.Products.Find(productId);
//                cart.Add(new CartItem
//                {
//                    ProductId = product.ProductId,
//                    Quantity = 1,
//                    Product = product // Link the Product directly
//                });
//            }
//            Session["Cart"] = cart;
//            return RedirectToAction("Cart");
//        }

//        public ActionResult Cart()
//        {
//            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
//            return View(cart);
//        }

//        [HttpPost]
//        public JsonResult UpdateCart(int productId, int quantity)
//        {
//            var cart = Session["Cart"] as List<CartItem>;
//            var item = cart.FirstOrDefault(c => c.ProductId == productId);

//            if (item != null)
//            {
//                if (quantity > 0)
//                {
//                    item.Quantity = quantity;
//                }
//                else
//                {
//                    cart.Remove(item);
//                }
//            }
//            Session["Cart"] = cart;

//            // Calculate total price for the item
//            var itemTotal = item != null ? item.Quantity * item.Product.Price : 0;

//            return Json(new
//            {
//                itemTotal = itemTotal,
//                cartTotal = cart.Sum(i => i.Quantity * i.Product.Price)
//            });
//        }
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using template.Models;

namespace template.Controllers
{
    public class ProductController : Controller
    {
        private MyWebsiteEntities db = new MyWebsiteEntities();

        public ActionResult Shop(int? categoryId)
        {
            List<Product> products;
            if (categoryId == null)
            {
                products = db.Products.ToList();
            }
            else
            {
                products = db.Products.Where(p => p.CategoryId == categoryId).ToList();
            }

            ViewBag.Categories = db.Categories.ToList();
            return View(products);
        }

        public ActionResult Details(int productId)
        {
            var product = db.Products.Find(productId);
            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }

        public ActionResult AddToCart(int productId)
        {
            if (Session["UserId"] == null)
            {

                return RedirectToAction("Login", "Account");

            }

            var userId = GetCurrentUserId();

            var cart = Session["Cart"] as List<CartItem> ?? new List<CartItem>();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                item.Quantity++;


                var cartItem = db.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.UserId == userId);
                if (cartItem != null)
                {
                    cartItem.Quantity = item.Quantity;
                }
            }
            else
            {
                var product = db.Products.Find(productId);
                var newCartItem = new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = 1,
                    Product = product,
                    UserId = userId // Link the cart item to the current user
                };

                cart.Add(newCartItem);
                db.CartItems.Add(newCartItem); // Add to the database
            }

            db.SaveChanges(); // Save changes to the database
            Session["Cart"] = cart;

            return RedirectToAction("Cart");
        }

        public ActionResult Cart()
        {
            if (Session["UserId"] == null)
            {

                return RedirectToAction("Login", "Account");

            }
            // Assuming you have a way to get the currently logged-in user's ID
            var userId = GetCurrentUserId();

            var cart = db.CartItems.Where(ci => ci.UserId == userId).ToList();
            Session["Cart"] = cart; // Store the cart in session for easy access

            return View(cart);
        }

        [HttpPost]
        public JsonResult UpdateCart(int productId, int quantity)
        {    


            var userId = GetCurrentUserId();

            var cart = Session["Cart"] as List<CartItem>;
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;

                    var cartItem = db.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.UserId == userId);
                    if (cartItem != null)
                    {
                        cartItem.Quantity = quantity;
                    }
                }
                else
                {
                    cart.Remove(item);


                    var cartItem = db.CartItems.FirstOrDefault(ci => ci.ProductId == productId && ci.UserId == userId);
                    if (cartItem != null)
                    {
                        db.CartItems.Remove(cartItem);
                    }
                }
            }

            db.SaveChanges();
            Session["Cart"] = cart;


            var itemTotal = item != null ? item.Quantity * item.Product.Price : 0;

            return Json(new
            {
                itemTotal = itemTotal,
                cartTotal = cart.Sum(i => i.Quantity * i.Product.Price)
            });
        }


        private int GetCurrentUserId()
        {


            return (int)Session["UserId"];
        }
    }
}
