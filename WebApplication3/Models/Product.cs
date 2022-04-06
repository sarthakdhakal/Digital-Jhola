using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebApplication3.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Display(Name = "Product")]
        [Required]
        public string ProductName { get; set; }

        [Display(Name = "Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "Unit In Stock")]
        public int ProductUnitInStock { get; set; }

        [Display(Name = "Price")]
        [Column(TypeName = "money")]
        public decimal ProductUnitPrice { get; set; }

 
        public string? ProductImgUrl { get; set; }
        
        
        public string? UserId { get; set; }
        public virtual User? User { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }
        public int? BrandId { get; set; }
        public virtual Brand? Brand { get; set; }
        
        public int? OfferId { get; set; }
        public virtual Offer Offer { get; set; }
        

    }
}
