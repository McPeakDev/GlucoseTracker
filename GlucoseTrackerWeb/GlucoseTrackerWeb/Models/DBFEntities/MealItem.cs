using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class MealItem
    {
        public int FoodId { get; set; }
        public string FoodName { get; set; }
        public int Carbs { get; set; }
    }
}
