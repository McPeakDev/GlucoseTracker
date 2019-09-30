using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlucoseTrackerWeb.Models.Entities
{
    public partial class MealItem
    {
        [Key]
        public int MealId { get; set; }
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int Carbs { get; set; }
    }
}
