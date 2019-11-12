using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public partial class PatientExercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExerciseId { get; set; }
        [ForeignKey("Patient")]
        public int UserId { get; set; }
        public float HoursExercised { get; set; }
        public DateTime TimeOfDay { get; set; }
        public virtual Patient Patient { get; set; }

        public override string ToString()
        {
            return $"Time: {TimeOfDay.ToLocalTime().ToShortTimeString()}, Hours Exercised: {HoursExercised}";
        }
    }
}
