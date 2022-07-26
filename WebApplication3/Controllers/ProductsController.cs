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
using QRCoder;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using WebApplication3.Enums;
using WebApplication3.Models;
using ZXing.QrCode;
using Image = WebApplication3.Models.Image;

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
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Index(int page = 1, string s = "")
        {
            page--;
            var applicationDbContext = _context.Products.Include(p => p.Category).Include(b=>b.Brand).Include(p => p.Offer).Where(e => e.ProductName.ToLower().Contains(s.ToLower()) || e.ProductDescription.ToLower().Contains(s.ToLower()));
            ViewBag.count = applicationDbContext.Count();
            ViewBag.countPerPage = CountPerPage;
            var result = applicationDbContext.Skip(page * CountPerPage).Take(CountPerPage);
            ViewBag.pagecount = result.Count();
            ViewBag.page = page + 1;
            ViewBag.search = s;
            ViewBag.pagination = pagination(applicationDbContext.Count(), page + 1, CountPerPage, s);
            return View(await result.ToListAsync());
        }
 
        [Authorize(Roles = "Admin, Seller")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,ProductDescription,ProductUnitInStock,ProductUnitPrice,CategoryId,BrandId")] Product product)
        {
            var files = HttpContext.Request.Form.Files;
            if (ModelState.IsValid)
            {
                int counter = 1;
                
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
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("ProductAdd", "true", option);
                return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            return View(product);
        }

        // GET: Products/Edit/5
        [Authorize(Roles = "Admin, Seller")]
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            return View(product);
        }

   
        [ValidateAntiForgeryToken]
        [HttpPost]
        [Authorize(Roles = "Admin, Seller")]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,ProductDescription,ProductUnitInStock,ProductUnitPrice,ProductImgUrl,SellerId,CategoryId")] Product product)
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
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("ProductEdit", "true", option);
                return RedirectToAction(nameof(Index));
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["BrandId"] = new SelectList(_context.Brands, "BrandId", "BrandName");
            return View(product);
        }

       

        [Authorize(Roles = "Admin, Seller")]
    
   
        public async Task<IActionResult> Delete(int? id)
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

            try
            {

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddSeconds(10);
            Response.Cookies.Append("ProductDelete", "true", option);
            return RedirectToAction(nameof(Index));
      
            
            }
            catch (Exception e)
            {
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("DeletionFail", "true", option);
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult ProductDetails(int? id)
        {
            if (id == null){
                return RedirectToAction("Error", "Home");
        }

            var url = HttpContext.Request.GetEncodedUrl().ToString();

            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);  
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            ViewBag.QrCodeUri = QrUri;
            var productUserId = _userManager.GetUserId(HttpContext.User);
            var product = _context.Products.Include(p => p.Category).Include(p => p.Brand).Include(ww => ww.Offer).Include(u=>u.User).FirstOrDefault(p => p.ProductId == id);
           ViewBag.Comments = _context.Comments.Where(c=>c.ProductId==id&& c.ApprovalStatus==1).Include(u => u.User).Include(u=>u.Replies).ThenInclude(r=>r.User).ToList();
           ViewBag.Reviews = _context.StarRatings.Where(c=>c.ProductId==id).Include(u => u.User).ToList();
           ViewBag.reviewgiver = _context.OrderItems.Any(u => u.Product.ProductId == id && u.Order.BuyerId==productUserId && u.Order.Status==Status.Delivered );
           ViewBag.images = _context.Images.Where(i => i.ProductId == id);
           if (_context.StarRatings.Any(c => c.ProductId == id))
           {


               ViewBag.Stars = Math.Round(_context.StarRatings.Where(c => c.ProductId == id).Average(s => s.Rating));
               ViewBag.CountStars = _context.StarRatings.Count(c => c.ProductId == id);
           }

           const int numOfRelated = 4;
            if (product is null)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                var relatedProducts = _context.Products.Include(ww => ww.Offer)
                    .Where(p => p.CategoryId == product.CategoryId && p.ProductId!= id).OrderByDescending(p=>p.ProductId).Take(4) ;
                ViewBag.product = product;
                int size = relatedProducts.Count();
                if (size < numOfRelated)
                {
                    for (int i = 0; i < numOfRelated - size; i++)
                    {
                        relatedProducts.ToList().Add(product);
                    }
                }
                return View(relatedProducts);
            }
        }
        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
