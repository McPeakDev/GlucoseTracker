using GlucoseAPI.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseTrackerWeb.Models.ViewModels
{
    public class PatientDataVM
    {

        public string FullName { get; set; }

        public List<PatientExercise> PatientExercises { get; set; }
        public List<PatientBloodSugar> PatientBloodSugars { get; set; }
        public List<PatientCarbohydrates> PatientCarbohydrates { get; set; }
    }
}
