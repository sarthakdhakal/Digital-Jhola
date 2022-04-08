using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;
using WebApplication3.Services;

namespace WebApplication3.Controllers
{
    public class CommentsController : Controller
    {
        private ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        public CommentsController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Create(int productId, string content)
        {
            if (ModelState.IsValid)
            {
                var comment = new Comment()
                {
                    ProductId = productId,
                    UserId = _userManager.GetUserId(User),
                    Content = content,
                };
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction("ProductDetails", "Products", new {id = productId});
            }
            TempData["Message"] = "The comment could not be added";
            return RedirectToAction("ProductDetails", "Products", new {id = productId});
        }
    }
}