﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Models;

namespace WebApplication3.Controllers
{
    [Authorize(Roles = "Admin, Seller")]
    public class OffersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;
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
                    p += "<li> <a class=\"pagination__link \" href=\"/Products/Index?page=" + i + "&s=" + s +
                         "\" onclick=\"ajaxRender(event)\">" + i + "</a> </li>";
                }
            }

            return p;
        }

        public OffersController(ApplicationDbContext context, IWebHostEnvironment environment, UserManager<User> userManager)
        {
            _context = context;
            _environment = environment;
            _userManager = userManager;
            
        }

     
        public async Task<IActionResult> Index(int page = 1, string s = "")
        {
            page--;
            var applicationDbContext = _context.Offers.Where(e => e.OfferName.ToLower().Contains(s.ToLower()));
            ViewBag.count = applicationDbContext.Count();
            ViewBag.countPerPage = CountPerPage;
            var result = applicationDbContext.Skip(page * CountPerPage).Take(CountPerPage);
            ViewBag.pagecount = result.Count();
            ViewBag.page = page + 1;
            ViewBag.search = s;
            ViewBag.pagination = pagination(applicationDbContext.Count(), page + 1, CountPerPage, s);
            return View(await result.ToListAsync());
        }

       
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers
                .FirstOrDefaultAsync(m => m.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }

            return View(offer);
        }


        public IActionResult Create()
        {
            ViewBag.UserId = _userManager.GetUserId(HttpContext.User);
            ViewBag.UserRoleAdmin = User.IsInRole("Admin");
            ViewBag.Products = _context.Products.ToList();
            return View();
        }

     
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfferId,OfferName,Sale,DateFrom,DateTo")] Offer offer,
            ICollection<int> MyProducts)
        {
            if (ModelState.IsValid)
            {
                _context.Add(offer);
                await _context.SaveChangesAsync();
                var prods = _context.Products.Where(p => MyProducts.Contains(p.ProductId)).ToList();
                prods.ForEach(p => p.Offer = offer);
                await _context.SaveChangesAsync();
                CookieOptions option = new CookieOptions();
                option.Expires = DateTime.Now.AddSeconds(10);
                Response.Cookies.Append("OfferAdd", "true", option);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }


         public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var offer = await _context.Offers.Include(e => e.Products).SingleOrDefaultAsync(e => e.OfferId == id);
            if (offer == null)
            {
                return NotFound();
            }

            ViewBag.Products = _context.Products.ToList();
            return View(offer);
        }

        [HttpPost]
      
        public async Task<IActionResult> Edit(int id, [Bind("OfferId,OfferName,Sale,DateFrom,DateTo")] Offer offer,
            ICollection<int> MyProducts)
        {
            if (id != offer.OfferId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
              
                    var oldOffers = _context.Products.Where(e => e.OfferId == id);
                    await oldOffers.ForEachAsync(a => a.OfferId = null);
                    await _context.SaveChangesAsync();
              
                    _context.Update(offer);
                    await _context.SaveChangesAsync();
                    var prods = _context.Products.Where(p => MyProducts.Contains(p.ProductId)).ToList();
                    prods.ForEach(p => p.Offer = offer);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OfferExists(offer.OfferId))
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
                Response.Cookies.Append("OfferEdit", "true", option);
                return RedirectToAction(nameof(Index));
            }

            return View(offer);
        }

     
   
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var oldsubscriper = _context.Products.Where(e => e.OfferId == id);
            await oldsubscriper.ForEachAsync(a => a.OfferId = null);
            await _context.SaveChangesAsync();
            var offer = await _context.Offers.FindAsync(id);
            
            _context.Offers.Remove(offer);
            await _context.SaveChangesAsync();
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddSeconds(10);
            Response.Cookies.Append("OfferRemove", "true", option);
            return RedirectToAction(nameof(Index));
        }

        private bool OfferExists(int id)
        {
            return _context.Offers.Any(e => e.OfferId == id);
        }
    }
}