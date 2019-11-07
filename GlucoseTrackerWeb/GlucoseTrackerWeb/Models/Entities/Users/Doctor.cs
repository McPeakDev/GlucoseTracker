///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerWeb/GlucoseTrackerWeb
//	File Name:         Doctor.cs
//	Description:       A Representation of an Doctor for Glucose Tracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Representation of a Doctor for Glucose Tracker
    /// </summary>
    public partial class Doctor : User
    {
        public Doctor()
        {
            Patients = new List<Patient>();
        }

        [Required]
        public int NumberOfPatients
        {
            get
            {
                return Patients.Count;
            }
        }

        public virtual ICollection<Patient> Patients { get; set; }
    }
}
