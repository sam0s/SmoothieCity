using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SmoothieCity.Models
{
    public partial class OrderItems
    {
        [Display(Name = "Order Item ID")]
        public int OrderItemsID { get; set; }

        public int OrderID { get; set; }
        public int SmoothieID { get; set; }


        // foreign keys
        public virtual Order Order { get; set; }
        public virtual Smoothies Smoothies { get; set; }
    }
}