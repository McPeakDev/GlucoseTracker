using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public struct PatientData
    {
        public UserCredentials UserCredentials { get; set; }
        public List<PatientExercise> PatientExercises { get; set; }
        public List<PatientBloodSugar> PatientBloodSugars { get; set; }
        public List<PatientCarbohydrates> PatientCarbohydrates { get; set; }

    }
}
