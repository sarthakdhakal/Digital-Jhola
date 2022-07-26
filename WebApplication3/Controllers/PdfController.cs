using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;
using WebApplication3.Models;
using Wkhtmltopdf.NetCore;


namespace WebApplication3.Controllers
{
    
    public class PdfController : Controller
    {
        private ApplicationDbContext DbContext;
        public PdfController(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            
           
        }

        public IActionResult ViewBillAsPdf(int id)
        {
           
            var items = DbContext.OrderItems.Where(a => a.OrderId == id).Include(p=>p.Product).Include(o=>o.Order).
            ThenInclude(o=>o.Buyer);
            
            return new ViewAsPdf("ViewBillAsPdf", items);  
        }  

    }
}
