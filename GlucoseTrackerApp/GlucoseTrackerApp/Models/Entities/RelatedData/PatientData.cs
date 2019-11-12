using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public class PatientData
    { 
       public PatientData()
        {
            PatientExercises = new List<PatientExercise>();
            PatientBloodSugars = new List<PatientBloodSugar>();
            PatientCarbohydrates = new List<PatientCarbohydrate>();
        }
        public List<PatientExercise> PatientExercises { get; set; }
        public List<PatientBloodSugar> PatientBloodSugars { get; set; }
        public List<PatientCarbohydrate> PatientCarbohydrates { get; set; }

    }
}
