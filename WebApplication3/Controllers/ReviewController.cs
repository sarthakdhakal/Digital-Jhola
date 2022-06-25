using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ReviewController : Controller
    {
        // GET
        private ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public ReviewController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Create(int productId, string content, float rating)
        {
            if (ModelState.IsValid)
            {
                var starRating = new StarRating()
                {
                    ProductId = productId,
                    UserId = _userManager.GetUserId(User),
                    Review = content,
                    Rating = rating,
                    CreatedOn = DateTime.Now
                };
                _context.StarRatings.Add(starRating);
                await _context.SaveChangesAsync();
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("ReplyAdded", "true", option);
                return RedirectToAction("ProductDetails", "Products", new {id = productId});
            }
            TempData["Message"] = "The rating could not be added";
            return RedirectToAction("ProductDetails", "Products", new {id = productId});
        }
    }
}