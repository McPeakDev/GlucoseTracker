///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         PatientExercise.cs
//	Description:       A Representation of a Patient's Exercise for Glucose Tracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Representation of a Patient's Exercise for Glucose Tracker
    /// </summary>
    public partial class PatientExercise
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ExerciseId { get; set; }
        [ForeignKey("Patient")]
        public int UserId { get; set; }
        public float HoursExercised { get; set; }
        public DateTime? TimeOfDay { get; set; }
        public virtual Patient Patient { get; set; }
    }
}
