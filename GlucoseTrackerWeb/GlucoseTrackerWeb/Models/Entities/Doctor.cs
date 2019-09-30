using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.Entities
{
    public partial class Doctor : User
    {
        public Doctor()
        {
            Patient = new List<Patient>();
        }

        public int DoctorId { get; set; }
        public int NumberOfPatients { get; set; }

        public ICollection<Patient> Patient { get; set; }
    }
}
