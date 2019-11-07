///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerWeb/GlucoseTrackerWeb
//	File Name:         PatientData.cs
//	Description:       A Data Structure for PatientData for Glucose Tracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Data Structure for PatientData for Glucose Tracker
    /// </summary>
    public struct PatientData
    {
        public List<PatientExercise> PatientExercises { get; set; }
        public List<PatientBloodSugar> PatientBloodSugars { get; set; }
        public List<PatientCarbohydrates> PatientCarbohydrates { get; set; }

    }
}
