using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SmoothieCity.Models
{
    public partial class CartItem
    {
        int id;
        public CartItem(Smoothies obj)
        {
            id = obj.SmoothieId;
        }



    }
}
