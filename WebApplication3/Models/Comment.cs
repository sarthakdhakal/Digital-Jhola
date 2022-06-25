using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class Comment
    {
        [Key]
        public int CommentId { get; set; }
        public int ProductId { get; set; }

        public virtual  Product  Product{ get; set; }
        public string Content { get; set; }

        public string UserId { get; set; }
        public int ApprovalStatus { get; set; }
        public virtual User User { get; set; }
        
        public DateTime? CreatedOn { get; set; }

        public ICollection<Reply> Replies { get; set; }
    }
}