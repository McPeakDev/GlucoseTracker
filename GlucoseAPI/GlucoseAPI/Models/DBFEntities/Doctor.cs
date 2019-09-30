using System;
using System.Collections.Generic;

namespace GlucoseAPI.Models.DBFEntities
{
    public partial class Doctor
    {
        public Doctor()
        {
            Patient = new HashSet<Patient>();
        }

        public int DoctorId { get; set; }
        public int NumberOfPatients { get; set; }

        public virtual User DoctorNavigation { get; set; }
        public virtual ICollection<Patient> Patient { get; set; }
    }
}
