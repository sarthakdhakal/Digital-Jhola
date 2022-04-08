using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Reply
    {
        [Key]
        public int ReplyId { get; set; }
        public int CommentId { get; set; }

        public virtual  Comment  Comment{ get; set; }
        public string Content { get; set; }

        public string UserId { get; set; }

        public virtual User User { get; set; }
        
        public DateTime? CreatedOn { get; set; }
        public int ProductId { get; set; }

        public virtual  Product  Product{ get; set; }
    }
}