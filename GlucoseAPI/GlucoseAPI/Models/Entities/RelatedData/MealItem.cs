///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         MealItem.cs
//	Description:       A Representation of a Meal Item for Glucose Tracker
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Representation of a Meal Item for Glucose Tracker
    /// </summary>
    public partial class MealItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MealId { get; set; }
        public MealType MealTime { get; set; }
        public string FoodName { get; set; }
        public int Carbs { get; set; }
    }
}
