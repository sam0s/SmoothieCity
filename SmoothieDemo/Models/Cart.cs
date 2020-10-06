using System;
using System.Collections.Generic;

namespace SmoothieDemo.Models
{
    public partial class Cart
    {
        public int ItemNumber { get; set; }
        public int? Sid { get; set; }

        public virtual Smoothies S { get; set; }
    }
}
