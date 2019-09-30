using System;
using System.Collections.Generic;

namespace GlucoseTrackerWeb.Models.Entities
{
    public partial class Login
    {
        public int LoginId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }

        public virtual User User { get; set; }
    }
}
