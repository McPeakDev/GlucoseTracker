///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         Patient.cs
//	Description:       A Representation of a Patient for Glucose Tracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Representation of a Patient for Glucose Tracker
    /// </summary>
    public partial class Patient : User
    {
        [ForeignKey("Doctor")]
        public int? DoctorId { get; set; }

        public virtual Doctor Doctor { get; set; }
        public virtual ICollection<PatientBloodSugar> PatientBloodSugars { get; set; }
        public virtual ICollection<PatientCarbohydrate> PatientCarbs { get; set; }
        public virtual ICollection<PatientExercise> PatientExercises { get; set; }
    }
}
