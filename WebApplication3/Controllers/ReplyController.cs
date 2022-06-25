using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ReplyController : Controller
    {
        private ApplicationDbContext _context;

        private UserManager<User> _userManager;
        // GET
        
        public ReplyController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        
            public async Task<IActionResult> Create(int productId, int commentId, string content)
            {
                if (ModelState.IsValid)
                {
                    var reply = new Reply()
                    {
                        CommentId = commentId,
                        UserId = _userManager.GetUserId(User),
                        Content = content,
                        ProductId = productId,
                        CreatedOn = DateTime.Now
                    };
                    _context.Replies.Add(reply);
                    await _context.SaveChangesAsync();
                    CookieOptions option = new CookieOptions();
                    option.Expires = DateTime.Now.AddSeconds(10);
                    Response.Cookies.Append("ReplyAdded", "true", option);
                    return RedirectToAction("ProductDetails", "Products", new {id = productId});
                }
                TempData["Message"] = "The reply could not be added";
                return RedirectToAction("ProductDetails", "Products", new {id = productId});
            }
        }
    }
