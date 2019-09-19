using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class PatientCarbohydrates
    {
        public int PatientId { get; set; }
        public int TotalCarbs { get; set; }
        public string MealName { get; set; }
        public int FoodCarbs { get; set; }
        public string Meal { get; set; }
        public DateTime? TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
