using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.Entities
{
    public partial class Patient : User
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual PatientBloodSugar PatientBloodSugar { get; set; }
        public virtual PatientCarbohydrates PatientCarbohydrates { get; set; }
        public virtual PatientExercise PatientExercise { get; set; }
    }
}
