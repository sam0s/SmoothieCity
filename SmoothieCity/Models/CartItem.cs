using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmoothieCity.Models
{
    public class CartItem
    {
        public Smoothies Smooth { get; set; }
        public int count { get; set; }
        public CartItem(Smoothies smoothies, int count)
        {
            Smooth = smoothies;
            this.count = count;
        }

      
    }
}
