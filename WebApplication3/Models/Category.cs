using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Display(Name = "Category")]
        [Required]
        public string CategoryName { get; set; }

        [Display(Name = "Description")]

        public string CategoryDescription { get; set; }

        [Display(Name = "Image")]
        public string? ImageUrl { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}
