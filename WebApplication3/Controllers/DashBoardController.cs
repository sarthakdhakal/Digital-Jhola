using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Enums;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class DashBoardController : Controller
    {
        private ApplicationDbContext DbContext;
        public DashBoardController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
        }
        // GET
        public IActionResult AdminDashBoard()
        {
            ViewBag.ItemSales = DbContext.OrderItems.Sum(e => e.Quantity);
            ViewBag.NewOrders = DbContext.Orders.Count(x=>x.Status==Status.Pending);
            ViewBag.NewOrders = DbContext.Orders.Count();
            ViewBag.TotalProducts = DbContext.Products.Count(e => e.ProductUnitInStock > 0);
            ViewBag.Users = DbContext.Users.Count();
            return View();
        }
    }
}