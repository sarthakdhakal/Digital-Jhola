using System.Collections.Generic;

namespace WebApplication3.Models
{
    public class CheckoutViewModel
    {
        public List<Product> CartProducts { get; set; }
        public List<int> CartProductIDs { get; set; }
        // public Dictionary<int, int> Order { get; set; }
        // public Order OrderData { get; set; }
    }
}