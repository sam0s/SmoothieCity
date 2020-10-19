using System;
using System.ComponentModel.DataAnnotations;

namespace SmoothieCity.Models
{
    public class Customer
    {
        [Display(Name = "Customer ID")]
        public int CustomerID { get; set; }

        [Display(Name ="Customer Name")]
        public string CustomerName { get; set; }

        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name ="Email")]
        public string Email { get; set; }

    }
}
