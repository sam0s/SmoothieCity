using System;
using System.Collections.Generic;

namespace SmoothieCity.Models
{
    public partial class Smoothies
    {
        public Smoothies()
        {
            
        }

        public int SmoothieId { get; set; }
        public string SmoothieName { get; set; }
        public int? SmoothieCalories { get; set; }
        public double? SmoothiePrice { get; set; }
        public string SmoothieImage { get; set; }

    }
}
