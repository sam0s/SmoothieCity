using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmoothieCity.Models
{
    public class ViewModel
    {
        public IEnumerable<SmoothieCity.Models.Smoothies> Smoothies { get; set; }
        public SmoothieCity.Models.OrderItems OrderItems { get; set; }
    }

}
