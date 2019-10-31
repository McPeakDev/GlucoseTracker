///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         PatientData.cs
//	Description:       A Data Structure for PatientData for Glucose Tracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Data Structure for PatientData for Glucose Tracker
    /// </summary>
    public struct PatientData
    {
        public ICollection<PatientExercise> PatientExercises { get; set; }
        public ICollection<PatientBloodSugar> PatientBloodSugars { get; set; }
        public ICollection<PatientCarbohydrates> PatientCarbohydrates { get; set; }

    }
}
