///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseTrackerWeb/GlucoseTrackerWeb
//	File Name:         Credentials.cs
//	Description:       A Data Structure to hold Credential information for a user.
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
    public class Credentials
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
