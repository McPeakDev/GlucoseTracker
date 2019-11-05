﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public partial class PatientCarbohydrates
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int  CarbId { get; set; }
        [ForeignKey("Patient")]
        public int UserId { get; set; }
        [ForeignKey("MealItem")]
        public int MealId { get; set; }
        public int FoodCarbs { get; set; }
        public MealItem Meal { get; set; }
        public DateTime TimeOfDay { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
