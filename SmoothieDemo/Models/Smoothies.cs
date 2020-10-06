using System;
using System.Collections.Generic;

namespace SmoothieDemo.Models
{
    public partial class Smoothies
    {
        public Smoothies()
        {
            Cart = new HashSet<Cart>();
        }

        public int SmoothieId { get; set; }
        public string SmoothieName { get; set; }
        public int? SmoothieCalories { get; set; }
        public double? SmoothiePrice { get; set; }
        public string SmoothieImage { get; set; }

        public virtual ICollection<Cart> Cart { get; set; }
    }
}
