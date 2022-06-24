using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication3.Models
{
    public class OrderItems
    {
        public int OrderItemsId { get; set; }
        public int ProductId { get; set; }
        public int OrderId { get; set; }

   
        public int Quantity { get; set; }
        [Column(TypeName ="money")]
        [Display(Name ="Price")]
        public decimal UnitPrice { get; set; }

      
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}