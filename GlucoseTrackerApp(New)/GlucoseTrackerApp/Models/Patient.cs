using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public partial class Patient : User
    {
        [Required]
        public int PatientId
        {
            get
            {
                return UserId;
            }
        }

        [Required]
        public int DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual PatientBloodSugar PatientBloodSugar { get; set; }
        public virtual PatientCarbohydrates PatientCarbohydrates { get; set; }
        public virtual PatientExercise PatientExercise { get; set; }
    }
}
