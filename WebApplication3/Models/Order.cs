using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebApplication3.Enums;

namespace WebApplication3.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [ForeignKey("Buyer")]
        public string BuyerId { get; set; }
        

        [Display(Name ="First name")]
        [Required]
        public string FirstName { get; set; }
        [Display(Name ="Last name")]
        [Required]
        public string LastName { get; set; }
        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required]
        [Display(Name = "phone")]
        [DataType(DataType.PhoneNumber)]
        public string Telephone { get; set; }

        public string Street { get; set; }
        public string City { get; set; }
        public string Province { get; set; }

        [Display(Name = "Date")]
        public DateTime OrderDate { get; set; }


        public Status Status { get; set; }


        [Column(TypeName ="money")]
        public decimal Discount { get; set; }

        [Column(TypeName = "money")]
        public decimal Taxes { get; set; }

        [Display(Name = "Total Price")]
        [Column(TypeName = "money")]
        public decimal TotalPrice { get; set; }
        [NotMapped]
        public decimal Sum { get; set; }

        public virtual ICollection<OrderItems> OrderItems { get; set; } = new HashSet<OrderItems>();

        public virtual User Buyer { get; set; }
        
    }
}