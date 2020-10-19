using System;
using System.ComponentModel.DataAnnotations;

namespace SmoothieCity.Models
{
    public class Order
    {

        [Display(Name = "Order ID")]
        public int OrderID { get; set; }

        [Display(Name ="Special Instructions")]
        public string SpecialInstructions { get; set; }

        [Display(Name = "Order Time")]
        [DataType(DataType.Date)]
        public DateTime OrderTime { get; set; }

        [Display( Name = "Pick-up Time")]
        [DataType(DataType.Date)]
        public DateTime PickUpTime { get; set; }

        public int CustomerID { get; set; }

        //foreign key to customer
        public virtual Customer Customer { get; set; }

    }
}
