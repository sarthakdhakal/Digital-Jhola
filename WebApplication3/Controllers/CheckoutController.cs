using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Enums;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class CheckoutController : Controller
    { 
        ApplicationDbContext db;
        CheckoutViewModel model;
        private  UserManager<User> _userManager;

        public CheckoutController(ApplicationDbContext _db, UserManager<User> userManager)
        {
            db = _db;
            model = new CheckoutViewModel();
            _userManager = userManager;
        }
        public IActionResult CheckoutOrderDetails()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            ViewBag.userData = _userManager.FindByIdAsync(userId).Result;
            string flag="";
            if (TempData.ContainsKey("name"))
                flag = TempData["name"].ToString();
            if (flag == "from shopping cart")
            {
                
                var cartProductCookie = Request.Cookies["CartProducts"];
                int quantity = 0;
                if (cartProductCookie != null && cartProductCookie != "")
                {
                    
                    model.CartProductIDs = cartProductCookie.Split('-').Select(x => int.Parse(x)).ToList();
                    model.CartProducts = db.Products.Include(w=>w.Offer).Where(p => model.CartProductIDs.Contains(p.ProductId)).ToList();
                    foreach (var product in model.CartProducts)
                    {
                        quantity = model.CartProductIDs.Count(x => x == product.ProductId);
                        model?.Order?.Add(product.ProductId, quantity);
                    }
                }
                TempData["name"] = "";
                TempData["name2"] = "from order details";
                
                return View(model);
            }
            else
            {
                return RedirectToAction("EditShoppingCart","ShoppingCart");
            }
        }
        [HttpPost]
        public IActionResult Checkout(CheckoutViewModel PersonalInfo)
        {
            string flag = "";
            if (TempData.ContainsKey("name2"))
                flag = TempData["name2"].ToString();
            if (flag == "from order details")
            {
                var cartProductCookie = Request.Cookies["CartProducts"];
                int quantity = 0;
                
                var sale = 0m;
                model.CartProductIDs = cartProductCookie.Split('-').Select(x => int.Parse(x)).ToList();
                model.CartProducts = db.Products.Include(ww=>ww.Offer).Where(p => model.CartProductIDs.Contains(p.ProductId)).ToList();
                if (cartProductCookie != null && cartProductCookie != "")
                {
                    //decimal _totalPrice = Convert.ToDecimal(model.CartProducts.Sum(x => x.ProductUnitPrice * model.CartProductIDs.Count(p => x.ProductId == p)));
                    decimal _totalPrice = 0m;
                    foreach (var item in model.CartProducts)
                    {
                            quantity = model.CartProductIDs.Count(x => x == item.ProductId);
                        if (item.OfferId == null)
                            _totalPrice += Convert.ToDecimal(item.ProductUnitPrice*quantity);
                        else
                        {
                            sale = item.ProductUnitPrice*(item.Offer.Sale / 100m);
                            _totalPrice += Convert.ToDecimal((item.ProductUnitPrice-sale)* quantity);
                        }
                    }
                    Order newOrder = new Order()
                    {
                        FirstName = PersonalInfo.OrderData.FirstName,
                        LastName = PersonalInfo.OrderData.LastName,
                        UserEmail = PersonalInfo.OrderData.UserEmail,
                        Telephone = PersonalInfo.OrderData.Telephone,
                        Street = PersonalInfo.OrderData.Street,
                        City = PersonalInfo.OrderData.City,
                        Province = PersonalInfo.OrderData.Province,
                        BuyerId = _userManager.GetUserId(User),
                        OrderDate = DateTime.Now,
                        Status = Status.Preparing,
                        Discount = 0.0m,
                        TotalPrice = _totalPrice
                    };
                    db.Orders.Add(newOrder);
                    db.SaveChanges();

                    var latestOrder = db.Orders.Max(x => x.OrderId);
                    decimal _UnitPrice = 0m;
                    foreach (var product in model.CartProducts)
                    {
                        quantity = model.CartProductIDs.Count(x => x == product.ProductId);
                        if (product.OfferId == null)
                            _UnitPrice = product.ProductUnitPrice;
                        else
                            _UnitPrice = product.ProductUnitPrice - (product.ProductUnitPrice * (product.Offer.Sale / 100m));

                        product.ProductUnitInStock = product.ProductUnitInStock - quantity;


                        model?.Order?.Add(product.ProductId, quantity);
                        db.OrderItems.Add(new OrderItems
                        {
                            OrderId = latestOrder,
                            ProductId = product.ProductId,
                            Quantity = quantity,
                            UnitPrice = _UnitPrice
                        });
                        db.SaveChanges();
                    }

                }
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("Checkout", "true", option);
                
                TempData["name2"] = "";
                Response.Cookies.Delete("CartProducts");
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("CheckoutOrderDetails", "Checkout");
            }
        }
    }
}
