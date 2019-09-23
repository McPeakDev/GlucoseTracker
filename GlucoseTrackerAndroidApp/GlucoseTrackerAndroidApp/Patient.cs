using System;
using System.Collections.Generic;
using System.Text;

namespace GlucoseTrackerAndroidApp
{
    public class Patient : User
    {
        public override int UserID { get; set; }
        public override string UserName { get; set; }
        public override string FirstName { get; set; }
        public override string LastName { get; set; }
        public override string Email { get; set; }
        public override string ContactNumber { get; set; }

        public Patient()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Doctor"/> class.
        /// </summary>
        /// <param name="ID">The ID used in the DataBase for this user.</param>
        /// <param name="UName">User Name.</param>
        /// <param name="FName">First Name.</param>
        /// <param name="LName">Last Name.</param>
        /// <param name="EMail">Email.</param>
        /// <param name="CNumb">Contact Number.</param>
        public Patient(int ID, string UName, string FName, string LName, string EMail, string CNumb)
        {
            this.UserID = ID;
            this.UserName = UName;
            this.FirstName = FName;
            this.LastName = LName;
            this.Email = EMail;
            this.ContactNumber = CNumb;
        }

    }
}
