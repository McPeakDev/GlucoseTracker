using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class PatientBloodSugar
    {
        public int PatientId { get; set; }
        public float LevelBefore { get; set; }
        public float LevelAfter { get; set; }
        public string Meal { get; set; }
        public DateTime? TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
    }
}
