using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlucoseAPI.Models.Entities
{
    public partial class PatientCarbohydrates
    {
        [Key]
        public int  CarbId { get; set; }
        public int PatientId { get; set; }
        public int TotalCarbs { get; set; }
        public string MealName { get; set; }
        public int FoodCarbs { get; set; }
        public string Meal { get; set; }
        public DateTime? TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
