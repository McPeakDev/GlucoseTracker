using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlucoseAPI.Models.Entities
{
    public partial class PatientBloodSugar
    {
        [Key]
        public int BloodId { get; set; }
        public int PatientId { get; set; }
        public float LevelBefore { get; set; }
        public float LevelAfter { get; set; }
        public string Meal { get; set; }
        public DateTime? TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
