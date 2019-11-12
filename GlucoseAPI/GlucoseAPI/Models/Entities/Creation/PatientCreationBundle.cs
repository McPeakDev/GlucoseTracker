///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         PatientCreationBundle.cs
//	Description:       A Data Structure to hold Patient information to be created
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Models.Entities
{
    public class PatientCreationBundle
    {
        public Patient Patient { get; set; }

        public string DoctorToken { get; set; }

        public string Password { get; set; }
    }
}
