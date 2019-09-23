using System.Text;
using System;
namespace GlucoseTrackerAndroidApp
{
    public static class Login
    {
        static Patient loggedInPatient = null;
        public static Patient LoginPatient(string username, string password)
        {
            Boolean success = AttemptLogin(username, password);
            return loggedInPatient;
        }

        private static Boolean AttemptLogin(string username, string password)
        {
            bool result = false;
            return result;
        }
    }
}