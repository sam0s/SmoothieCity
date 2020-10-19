using System;
using System.ComponentModel.DataAnnotations;

namespace SmoothieCity.Models
{
    public class OrderItems
    {
        [Display(Name = "Order Item ID")]
        public int OrderItemID { get; set; }

        public int OrderID { get; set; }
        public int SmoothieID { get; set; }


        // foreign keys
        public virtual Order Order { get; set; }
        public virtual Smoothies Smoothies { get; set; }
    }
}
