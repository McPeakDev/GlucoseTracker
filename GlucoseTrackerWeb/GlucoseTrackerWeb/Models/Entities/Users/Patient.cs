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
<<<<<<< Updated upstream:GlucoseTrackerWeb/GlucoseTrackerWeb/Models/Entities/Users/Patient.cs

        public int? DoctorId { get; set; }
=======
        public int DoctorId { get; set; }
>>>>>>> Stashed changes:GlucoseTrackerWeb/GlucoseTrackerWeb/Models/Entities/Patient.cs

        public virtual Doctor Doctor { get; set; }
        public virtual List<PatientBloodSugar> PatientBloodSugars { get; set; }
        public virtual List<PatientCarbohydrates> PatientCarbohydrates { get; set; }
        public virtual List<PatientExercise> PatientExercises { get; set; }
    }
}
