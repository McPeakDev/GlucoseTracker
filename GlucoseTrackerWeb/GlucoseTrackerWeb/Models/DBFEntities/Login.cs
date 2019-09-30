using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.DBFEntities
{
    public partial class Login
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }

        public virtual User EmailNavigation { get; set; }
        public virtual User User { get; set; }
    }
}
