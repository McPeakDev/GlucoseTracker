///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         PatientBloodSugar.cs
//	Description:       A Representation of a Patient's Blood Sugar for Glucose Tracker
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
    /// A Representation of a Patient's Blood Sugar for Glucose Tracker
    /// </summary>
    public partial class PatientBloodSugar
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BloodId { get; set; }
        [ForeignKey("Patient")]
        public int UserId { get; set; }
        [ForeignKey("MealItem")]
        public int MealId { get; set; }
        public float LevelBefore { get; set; }
        public float LevelAfter { get; set; }
        public DateTime TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual MealItem Meal { get; set; }

    }
}
