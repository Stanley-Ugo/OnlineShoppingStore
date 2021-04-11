using OnlineShoppingStore.DAL;
using OnlineShoppingStore.Models.Home;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OnlineShoppingStore.Controllers
{
    public class HomeController : Controller
    {
        dbMyOnlineShoppingEntities context = new dbMyOnlineShoppingEntities();
        public ActionResult Index(string search, int? page)
        {
            HomeIndexViewModel model = new HomeIndexViewModel();
            return View(model.CreateModel(search, 4, page));
        }

        public ActionResult AddToCart(int productId)
        {
            if(Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();

                var product = context.Tbl_Product.Find(productId);

                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });

                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];

                var product = context.Tbl_Product.Find(productId);

                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });

                Session["cart"] = cart;
            }

            return RedirectToAction("Index");
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}