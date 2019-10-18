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
            Patients = new List<Patient>();
        }

        [Required]
        public int NumberOfPatients
        {
            get
            {
                return Patients.Count;
            }
        }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
