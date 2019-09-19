using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class PatientExercise
    {
        public int PatientId { get; set; }
        public int HoursExercised { get; set; }
        public DateTime? TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
