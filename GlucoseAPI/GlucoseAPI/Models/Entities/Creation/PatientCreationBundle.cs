using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Models.Entities
{
    public class PatientCreationBundle
    {
        public Patient Patient { get; set; }

        private string _password;
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = HashPassword(value);
            }
        }
    }
}
