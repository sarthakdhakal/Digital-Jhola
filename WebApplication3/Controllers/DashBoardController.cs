using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Enums;
using WebApplication3.Models;

namespace WebApplication3.Controllers

{
    public class DashBoardController : Controller
    {
       
        private ApplicationDbContext DbContext;
        private  UserManager<User> _userManager;
       
       
     
        public DashBoardController(ApplicationDbContext dbContext,UserManager<User> userManager)
        {
            DbContext = dbContext;
            _userManager = userManager;
           
        }
        // GET
        [Authorize(Roles = "Admin")]
        public IActionResult AdminDashBoard()
        
        {
            ViewBag.TotalProducts = DbContext.Products.Count(e => e.ProductUnitInStock > 0);
            ViewBag.NewOrders = DbContext.Orders.Count(x=>x.Status==Status.Preparing);
            ViewBag.TotalOrdersSold = DbContext.Orders.Count(x=>x.Status==Status.Delivered);
            ViewBag.Users = DbContext.Users.Count();
            ViewBag.Admins = _userManager.GetUsersInRoleAsync("Admin").Result;
            ViewBag.adminCount = ViewBag.Admins.Count;
            ViewBag.Buyers = _userManager.GetUsersInRoleAsync("Buyer").Result;
            ViewBag.buyerCount = ViewBag.Buyers.Count;
            ViewBag.Seller = _userManager.GetUsersInRoleAsync("Seller").Result;
            ViewBag.sellerCount = ViewBag.Seller.Count;
            ViewBag.TotalSales = DbContext.Orders.Sum(x => x.TotalPrice);
            return View();
        }
        [Authorize(Roles = "Buyer")]
        public IActionResult BuyerDashboard()
        {
            string userId = _userManager.GetUserId(HttpContext.User);
            var orders = DbContext.Orders.Where(o => o.Buyer.Id == userId);
            
           
            return View(orders);
        }
        [Authorize(Roles = "Seller")]
        public IActionResult SellerDashboard()
        {
            return RedirectToAction("Index","Products");

      
        }
        [Authorize(Roles = "Buyer")]
        public IActionResult BuyerOrderItemsDashboard( int? id)
        {

          
                var items = DbContext.OrderItems.Where(a => a.OrderId == id).Include(p=>p.Product);
            
                
                
                return View(items);
        }

      
    }
}