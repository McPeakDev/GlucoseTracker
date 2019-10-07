using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public partial class Doctor : User
    {
        public Doctor()
        {
            Patient = new List<Patient>();
        }

        [Required]
        public int DoctorId
        {
            get
            {
                return UserId;
            }
        }

        [Required]
        public int NumberOfPatients
        {
            get
            {
                return Patient.Count;
            }
        }

        public ICollection<Patient> Patient { get; set; }
    }
}
