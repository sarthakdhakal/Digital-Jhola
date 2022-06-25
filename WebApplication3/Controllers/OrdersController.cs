using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Enums;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = "Admin, Seller")]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment environment;
        public OrdersController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            this.environment = environment;
        }
        // GET
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Status status)
        {
                    
            if (ModelState.IsValid)
            {
                if (status== Status.Preparing)
                {
                    status = Status.Pending;
                }
                else if (status== Status.Pending)
                {
                    status = Status.Shipping;
                }
                else if (status == Status.Shipping)
                {
                    status = Status.Delivered;
                }
                
            }
            var order =  _context.Orders
                .FirstOrDefault(m => m.OrderId == id);
            order.Status = status;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
                }
               
            }
            
        }
 
