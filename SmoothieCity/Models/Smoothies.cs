using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmoothieCity.Models
{
    public partial class Smoothies
    {
        public Smoothies()
        {
            
        }

        [Display(Name = "Smoothie ID")]
        public int SmoothieId { get; set; }

        [Display(Name = "Smoothie Name")]
        public string SmoothieName { get; set; }

        [Display(Name = "Total Calories")]
        public int? SmoothieCalories { get; set; }

        [Display(Name = "Total Price")]
        public double SmoothiePrice { get; set; }

        [Display(Name = "Image")]
        public string SmoothieImage { get; set; }

        [Display(Name = "Smoothie Ingredients")]
        public string SmoothieIngredients { get; set; }

    }
}
