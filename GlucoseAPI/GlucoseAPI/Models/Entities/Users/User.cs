///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         User.cs
//	Description:       A Representation of a User for Glucose Tracker
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
    /// A Representation of a User for Glucose Tracker
    /// </summary>
    public abstract class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required]
        [StringLength(150)]
        public string FirstName { get; set; }
        [StringLength(150)]
        public string MiddleName { get; set; }
        [Required]
        [StringLength(150)]
        public string LastName { get; set; }
        [StringLength(255)]
        public string Email { get; set; }
        [Required]
        [StringLength(11)]
        public string PhoneNumber { get; set; }
    }
}
