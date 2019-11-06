﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public partial class PatientBloodSugar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BloodId { get; set; }
        [ForeignKey("Patient")]
        public int UserId { get; set; }
        [ForeignKey("MealItem")]
        public int MealId { get; set; }
        public float LevelBefore { get; set; }
        public float LevelAfter { get; set; }
        public DateTime TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual MealItem Meal { get; set; }

    }
}
