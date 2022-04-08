using Microsoft.Build.Framework;

namespace WebApplication3.Models
{
    public class InputCommentModel
    {
        public int ProductId { get; set; }
        
        [Required]
        public string Content { get; set; }
    }
}