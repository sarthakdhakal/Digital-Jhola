using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            var userId = _userManager.GetUserId(User);
            var comment = _context.Comments.Where(c => c.Product.UserId ==userId &&c.ApprovalStatus==0).Include(c => c.Product)
                .Include(c => c.User);
            
            return View(comment);
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
                    ApprovalStatus = 0,
                    CreatedOn = DateTime.Now
                };
                _context.Comments.Add(comment);
                await _context.SaveChangesAsync();
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("CommentAdd", "true", option);
                return RedirectToAction("ProductDetails", "Products", new {id = productId});
            }
            TempData["Message"] = "The comment could not be added";
            return RedirectToAction("ProductDetails", "Products", new {id = productId});
        }

    [Authorize(Roles = "Seller")]
        public async Task<IActionResult> ApproveComment(int? id)
        {
            var comment = await _context.Comments.FindAsync(id);
            comment.ApprovalStatus = 1;
            _context.Update(comment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteComment(int? id)
        {
            var comment = await _context.Comments.FindAsync(id);

            if (comment == null)
            {
                return NotFound();
            }

            _context.Comments.Remove(comment);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}