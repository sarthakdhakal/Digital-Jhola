using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication3.Models;
//using Ecommerce.Data;
using Microsoft.EntityFrameworkCore;
using ReflectionIT.Mvc.Paging;
using WebApplication3.Models;


namespace Ecommerce.Controllers
{
    public class HomeController : Controller
    {
        // private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}
        ApplicationDbContext db;

        public HomeController(ApplicationDbContext _db)
        {
            db = _db;
        }

        public IActionResult Index()
        {
            ViewBag.categories = db.Categories.ToList();
            ViewBag.brands = db.Brands.ToList();
            ViewBag.products = db.Products.Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductId).Take(10)
                .ToList();
            ViewBag.sliders = db.Images.ToList();
            return View();
        }

        public IActionResult getAllProducts(int _categoryId, int _brandId, string search, int pageNumber = 1,
            int order = 1)
        {
            var products = db.Products.ToList();
            ViewBag.order = order;
            ViewBag.categories = db.Categories.ToList();
            ViewBag.Brands = db.Brands.ToList();
            ViewBag.pageNumber = pageNumber;
            var ShowingProducts = products;

            if (search != null)
            {
                ViewBag.search = search;
                if (_categoryId != 0)
                {
                    ViewBag.categoryId = _categoryId;
                    if (order == 1)
                    {

                        products = db.Products.Where(product => product.CategoryId == _categoryId && (product.ProductName.Contains(search) || (product.Brand!= null && product.Brand.BrandName.Contains(search)))
                                ).Include(ww => ww.Offer).OrderBy(ww => ww.ProductUnitPrice).ToList();
                    }
                    else if (order == 2)
                    {
                        products = db.Products.Where(product => product.CategoryId == _categoryId && (product.ProductName.Contains(search) || (product.Brand!= null && product.Brand.BrandName.Contains(search)))
                        ).Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductUnitPrice).ToList();

                    }
                    
                    ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                    ViewBag.Allproducts = products.ToList();


                }
                else if (_brandId != 0)
                {
                    ViewBag.brandId = _brandId;
                    if (order == 1)
                    {
                        products = db.Products
                            .Where(product =>
                                product.BrandId == _brandId && (product.ProductName.Contains(search) || (product.Category!= null && product.Category.CategoryName.Contains(search)))
                            )
                            .Include(ww => ww.Offer).OrderBy(ww => ww.ProductUnitPrice).ToList();
                    }
                    else if (order == 2)
                    {
                        products = db.Products
                            .Where(product =>
                                product.BrandId == _brandId && (product.ProductName.Contains(search) || (product.Category!= null && product.Category.CategoryName.Contains(search))))
                            .Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductUnitPrice).ToList();
                    }

                    ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                    ViewBag.Allproducts = products.ToList();
                }
                
                else
                {
                    

                    if (order == 1)
                    {
                        products = db.Products.Where(product => product.ProductName.Contains(search) || (product.Brand!= null && product.Brand.BrandName.Contains(search)) || (product.Category!= null && product.Category.CategoryName.Contains(search)))
                            .Include(ww => ww.Offer).OrderBy(ww => ww.ProductUnitPrice).ToList();
                    }
                    else if (order == 2)
                    {
                        products = db.Products.Where(product => product.ProductName.Contains(search) || (product.Brand!= null && product.Brand.BrandName.Contains(search)) || (product.Category!= null && product.Category.CategoryName.Contains(search)))
                            .Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductUnitPrice).ToList();
                    }
                    ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                    ViewBag.Allproducts = products.ToList();

                }

            }
            else
            {
                if (_categoryId != 0)
                {
                    ViewBag.categoryId = _categoryId;
                    if (order == 1)
                    {

                        products = db.Products.Where(product => product.CategoryId == _categoryId ).Include(ww => ww.Offer).OrderBy(ww => ww.ProductUnitPrice).ToList();
                    }
                    else if (order == 2)
                    {
                        products = db.Products.Where(product => product.CategoryId == _categoryId ).Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductUnitPrice).ToList();

                    }
                    ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                    ViewBag.Allproducts = products.ToList();


                }
                else if (_brandId != 0)
                {
                    ViewBag.brandId = _brandId;
                    if (order == 1)
                    {
                        products = db.Products.Where(product => product.BrandId == _brandId)
                            .Include(ww => ww.Offer).OrderBy(ww => ww.ProductUnitPrice).ToList();
                    }
                    else if (order == 2)
                    {
                        products = db.Products.Where(product => product.BrandId == _brandId)
                            .Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductUnitPrice).ToList();
                    }

                    ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                    ViewBag.Allproducts = products.ToList();
                }
                else
                {
                    if (order == 1)
                    {

                        products = db.Products.Include(ww => ww.Offer).OrderBy(ww => ww.ProductUnitPrice).ToList();
                    }
                    else if (order == 2)
                    {
                        products = db.Products.Include(ww => ww.Offer).OrderByDescending(ww => ww.ProductUnitPrice).ToList();

                    }
                    ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                    ViewBag.Allproducts = products.ToList();

                }
            }
            return PartialView(ShowingProducts);
        }
        public IActionResult Shop(int _categoryId, int _brandId, int pageNumber = 1)
        {
            var products = db.Products.ToList();
            ViewBag.categories = db.Categories.ToList();
            ViewBag.Brands = db.Brands.ToList();
            var ShowingProducts = products;
            if (_categoryId != 0)
            {
                ViewBag.categoryId = _categoryId;
                products = db.Products.Where(product => product.CategoryId == _categoryId).Include(ww => ww.Offer).ToList();
                ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                ViewBag.Allproducts = products.ToList();


            } 
            if (_brandId != 0)
            {
                ViewBag.brandId = _brandId;
                products = db.Products.Where(product => product.BrandId == _brandId).Include(ww => ww.Offer).ToList();
                ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                ViewBag.Allproducts = products.ToList();


            }
            else
            {
                products = db.Products.Include(ww => ww.Offer).ToList();
                ShowingProducts = products.Skip((pageNumber - 1) * 6).Take(6).ToList();
                ViewBag.Allproducts = products.ToList();

            }
            return View(ShowingProducts);
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}