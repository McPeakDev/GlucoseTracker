using System;
using System.Collections.Generic;
using System.Text;

namespace GlucoseTrackerAndroidApp
{ 
    public abstract class User
    {
        abstract public int UserID { get; set; }
        abstract public string UserName { get; set; }
        abstract public string FirstName { get; set; }
        abstract public string LastName { get; set; }
        abstract public string Email { get; set; }
        abstract public string ContactNumber { get; set; }

    }
}
