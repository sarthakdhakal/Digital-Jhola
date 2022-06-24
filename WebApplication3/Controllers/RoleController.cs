using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class RoleController : Controller
    {
        private readonly UserManager<User> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;
        
        public RoleController(UserManager<User> userManager,ApplicationDbContext context, RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context ;
        }
        // GET
   
        public IActionResult Index()
        {

            return View(_context.Roles.ToList());
        }

        public ActionResult AssignUserToRole()
        {
            
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.Roles = new SelectList(_context.Roles, "Name", "Name");
            return View();
        }
        [HttpPost]
        public async Task<ActionResult> AssignUserToRole(string Id, string Name)
        {
            var user = await _userManager.FindByIdAsync(Id);
            await _userManager.AddToRoleAsync(user, Name);
            ViewBag.Users = new SelectList(_context.Users, "Id", "UserName");
            ViewBag.Roles = new SelectList(_context.Roles, "Name", "Name");

            return RedirectToAction(nameof(Index));
        }


        public ActionResult Create()
        {
            return View();
        }

        // POST: Roles/Create
        [HttpPost]
        public async Task<ActionResult> Create(string firstName)
        {

            // TODO: Add insert logic here
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(firstName));
                
                    return RedirectToAction("Index");
                
            }
            return View(firstName);
            
                
        }

        public ActionResult Edit(string Id)
        {
            return PartialView(_context.Roles.SingleOrDefault(c=>c.Id==Id));
        }

        [HttpPost]
        public ActionResult Edit(string firstName,string Id)
        {
            var role = _context.Roles.SingleOrDefault(c =>
                string.Equals(c.Id.ToLower(), Id.ToLower(), StringComparison.Ordinal));
            role.Name = firstName;
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
           
           // return Json(new { status = 0 });

        }

        public ActionResult Delete(string Id)
        {
            var role = _context.Roles.SingleOrDefault(c => c.Id.ToLower() == Id.ToLower());
            _context.Roles.Remove(role);
            _context.SaveChanges();
            return Json(new { status = 1 }); 

        }


    }


}
