///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerWeb/GlucoseTrackerWeb
//	File Name:         DoctorCreationBundle.cs
//	Description:       A Bundle of Doctor Data
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using System.ComponentModel.DataAnnotations;
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// A Bundle of Doctor Data
    /// </summary>
    public class DoctorCreationBundle
    {
        [Required(ErrorMessage = "User Already Exists")]
        public Doctor Doctor { get; set; }

        private string _password;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number")]
        [Required(ErrorMessage = "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number", AllowEmptyStrings = false)]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (!(value is null))
                {
                    _password = value;
                }
            }
        }
    }
}
