#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment environment;
        private readonly UserManager<User> _userManager;
        private static int CountPerPage { get; set; } = 10;
        public static string pagination(int totalRecords, int pageNum, int pageCapacity, string s)
        {
            string p = "";


            int numOfPages = (totalRecords + pageCapacity - 1) / pageCapacity;
            for (var i = 1; i <= numOfPages; i++)
            {
                if (i == pageNum)
                {
                    p += "<li> <a class=\"pagination__link pagination__link--active\" >" + i + "</a> </li>";
                }
                else
                {
                    p += "<li> <a class=\"pagination__link \" href=\"/Products/Index?page=" + i + "&s=" + s + "\" onclick=\"ajaxRender(event)\">" + i + "</a> </li>";
                }
            }
            return p;
        }
        public ProductsController(ApplicationDbContext context, IWebHostEnvironment environment,UserManager<User> userManager)
        {
            _context = context;
            this.environment = environment;
            _userManager = userManager;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,ProductUnitInStock,ProductUnitPrice,CategoryId")] Product product)
        {
            // var files = HttpContext.Request.Form.Files;
            if (ModelState.IsValid)
            {
                int counter = 1;
                var files = HttpContext.Request.Form.Files;
                foreach (var image in files)
                {
                    
                    
                    if (image != null && image.Length > 0)
                    {
                        var file = image;
                
                        
                        if (file.Length > 0)
                        {
                            var imageData = new Image();
                            if (counter == 1)
                            {
                                product.ProductImgUrl =@$"{Guid.NewGuid().ToString().Replace("-", "").Replace(" ", "")}.png";
                                var productUserId = _userManager.GetUserId(HttpContext.User);
                                if (productUserId != null)
                                    product.UserId = productUserId;
                                _context.Products.Add(product);
                                 await _context.SaveChangesAsync();
                                 imageData.ImagePath = product.ProductImgUrl;
                                imageData.ProductId = product.ProductId;
                                var filePath = Path.Combine(environment.WebRootPath, "Product/images",
                                    product.ProductImgUrl);
                                using (var stream = System.IO.File.Create(filePath))
                                {
                                    await file.CopyToAsync(stream);
                                }
                            }
                            else
                                {
                                    
                                    var imgVal = @$"{Guid.NewGuid().ToString().Replace("-", "").Replace(" ", "")}.png";
                                    var productId = product.ProductId;
                                
                                    var filePath = Path.Combine(environment.WebRootPath, "Product/images",
                                        imgVal);
                                    imageData.ImagePath = imgVal;
                                    imageData.ProductId = productId;
                                    using (var stream = System.IO.File.Create(filePath))
                                    {
                                        await file.CopyToAsync(stream);
                                    }
                                }
                            _context.Images.Add(imageData);
                            await _context.SaveChangesAsync();
                        }   
                    }
                    counter++;
                }
                return RedirectToAction(nameof(Index));
            }

           
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

   
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescrition,ProductUnitInStock,ProductUnitPrice,ProductImgUrl,SellerId")] Product product)
        {   
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
