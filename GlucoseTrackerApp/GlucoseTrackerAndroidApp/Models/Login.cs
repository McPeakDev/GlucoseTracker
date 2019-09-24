using System.Text;
using System;
using GlucoseTrackerWeb.Models;

namespace GlucoseTrackerAndroidApp
{
    public static class Login
    {
        static Patient loggedInPatient = null;
        public static Patient LoginPatient(Credentials creds)
        {
            Boolean success = AttemptLogin(creds);
            return loggedInPatient;
        }

        private static Boolean AttemptLogin(Credentials creds)
        {
            bool result = false;
            return result;
        }
    }
}