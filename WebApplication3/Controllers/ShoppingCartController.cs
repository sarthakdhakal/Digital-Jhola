using Microsoft.AspNetCore.Mvc;

namespace WebApplication3.Controllers
{
    public class ShoppingCartController : Controller
    {

        // GET
        public IActionResult EditShoppingCart()
        {
            ViewBag.UserRoleAdmin = User.IsInRole("Buyer");    
            return View();
        }

        public IActionResult UpdateCart()
        {
            return ViewComponent("ViewCart");
        }
    }
}