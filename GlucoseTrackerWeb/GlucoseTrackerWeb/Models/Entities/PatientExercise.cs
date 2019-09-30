using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GlucoseTrackerWeb.Models.Entities
{
    public partial class PatientExercise
    {
        [Key]
        public int ExerciseId { get; set; }
        public int PatientId { get; set; }
        public int HoursExercised { get; set; }
        public DateTime? TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
