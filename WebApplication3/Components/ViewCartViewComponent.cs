using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Components
{
    public class ViewCartViewComponent:ViewComponent
    {
        private readonly ApplicationDbContext _context;
        CheckoutViewModel checkoutViewModel;

        public ViewCartViewComponent(ApplicationDbContext context)
        {
            _context = context;
            checkoutViewModel = new CheckoutViewModel();
        }
        
        public IViewComponentResult Invoke()
        {
            var cartProductCookie = Request.Cookies["CartProducts"];
            int quantity = 0;
            if (cartProductCookie != null && cartProductCookie != "")
            {
                checkoutViewModel.CartProductIDs = cartProductCookie.Split('-').Select(x => int.Parse(x)).ToList();
                checkoutViewModel.CartProducts = _context.Products.Include(ww => ww.Offer).Where(p => checkoutViewModel.CartProductIDs.Contains(p.ProductId)).ToList();
                foreach (var product in checkoutViewModel.CartProducts)
                {
                    quantity = checkoutViewModel.CartProductIDs.Count(x => x == product.ProductId);
                    if (product.ProductUnitInStock != 0)
                        checkoutViewModel?.Order?.Add(product.ProductId, quantity);
                }
                TempData["name"] = "from shopping cart";

            }
            return View(checkoutViewModel);
        }
    }
}