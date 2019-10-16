using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public struct PatientData
    {
        UserCredentials UserCredentials { get; set; }
        IQueryable<PatientExercise> PatientExercises { get; set; }
        IQueryable<PatientBloodSugar> PatientBloodSugars { get; set; }
        IQueryable<PatientCarbohydrates> PatientCarbohydrates { get; set; }

    }
}
