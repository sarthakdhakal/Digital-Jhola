using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class ImageSellerVerify
    {
        [Key]
        public int ImageSellerVerifyId { get; set; }

        [Display(Name = "Image path")]
        [Required]
        public string ImagePath { get; set; }

        public string SellerId { get; set; }
        public virtual User Seller { get; set; }
    }
}