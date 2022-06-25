using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Brand
    {
        [Key]
        public int BrandId { get; set; }
        [Required]
        [Display(Name = "Brand name")]
        public string BrandName{ get; set; }
        [Display(Name = "Description")]

        public string BrandDescription { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }
        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}