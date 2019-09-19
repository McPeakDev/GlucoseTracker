using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class Patient
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual User PatientNavigation { get; set; }
        public virtual PatientBloodSugar PatientBloodSugar { get; set; }
        public virtual PatientCarbohydrates PatientCarbohydrates { get; set; }
        public virtual PatientExercise PatientExercise { get; set; }
    }
}
