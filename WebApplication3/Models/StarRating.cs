

using System;

namespace WebApplication3.Models
{
    public class StarRating
    {
        public int StarRatingId { get; set; }
        public float Rating { get; set; }
        public string? Review { get; set; }
        public string UserId { get; set; }
        public virtual User User { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public DateTime? CreatedOn { get; set; }
            
    }
}