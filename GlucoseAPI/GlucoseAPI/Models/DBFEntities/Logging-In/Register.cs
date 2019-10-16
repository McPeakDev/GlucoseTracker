using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public class Register
    {
        public Credentials Credentials{ get; set; }
        public string Token { get; set; }
        public virtual Patient Patient { get; set; }
        public virtual Doctor Doctor { get; set; }

    }
}
