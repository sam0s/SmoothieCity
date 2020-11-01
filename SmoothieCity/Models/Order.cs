using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;


namespace SmoothieCity.Models
{
    public partial class Order
    {
        public Order()
        {

        }

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name ="Special Instructions")]
        public string SpecialInstructions { get; set; }

        [Display(Name = "Order Time")]
        [DataType(DataType.Text)]
        public String OrderTime { get; set; }

        [Display( Name = "Pick-up Time")]
        [DataType(DataType.Text)]
        public String PickUpTime { get; set; }

        [Display(Name = "Submitted")]
        public bool Submitted { get; set; }
        public String CustomerID { get; set; }

        //foreign key to customer
        public virtual AspNetUsers Customer { get; set; }

    }
}
