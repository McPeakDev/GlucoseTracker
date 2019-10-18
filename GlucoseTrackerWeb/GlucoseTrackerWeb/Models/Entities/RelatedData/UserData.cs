using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public class UserData
    {
        public UserCredentials UserCredentials { get; set; }
        public string Token { get; set; }
        public virtual Doctor Doctor { get; set; }
    }
}
