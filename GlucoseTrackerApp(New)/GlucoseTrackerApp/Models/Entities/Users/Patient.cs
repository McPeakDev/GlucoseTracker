using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public partial class Patient : User
    {
        public int? DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual List<PatientBloodSugar> PatientBloodSugars { get; set; }
        public virtual List<PatientCarbohydrates> PatientCarbs { get; set; }
        public virtual List<PatientExercise> PatientExercises { get; set; }
    }
}
