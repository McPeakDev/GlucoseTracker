///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerApp/GlucoseTrackerApp
//	File Name:         PatientCarbohydrate.cs
//	Description:       A Representation of a Patient's Carbohydrates for Glucose Tracker
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
    /// A Representation of a Patient's Carbohydrates for Glucose Tracker
    /// </summary>
    public partial class PatientCarbohydrate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int  CarbId { get; set; }
        [ForeignKey("Patient")]
        public int UserId { get; set; }
        [ForeignKey("MealItem")]
        public int MealId { get; set; }
        public int FoodCarbs { get; set; }
        public DateTime TimeOfDay { get; set; }

        public virtual Patient Patient { get; set; }
        public virtual MealItem Meal { get; set; }

        public override string ToString()
        {
            return $"Time: {TimeOfDay.ToLocalTime().ToShortTimeString()}, Carbs: {FoodCarbs}, Meal: {Meal.FoodName}";
        }
    }
}
