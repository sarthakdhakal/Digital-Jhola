using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Image  
    {

        [Key]
        public int ImageId { get; set; }

        [Display(Name = "Image path")]
        [Required]
        public string ImagePath { get; set; }

        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}
