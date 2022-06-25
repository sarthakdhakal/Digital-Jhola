using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    public class Offer
    {
        public int OfferId { get; set; }

        [Display(Name ="Offer")]
        public string OfferName { get; set; }

        [Display(Name = "Sale %")]
        [Required]
        [Range(1,99)]
        [Column(TypeName = "money")]
        public double Sale { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        

        public DateTime? DateFrom { get; set; }

       
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime? DateTo { get; set; }

        public virtual ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }
}