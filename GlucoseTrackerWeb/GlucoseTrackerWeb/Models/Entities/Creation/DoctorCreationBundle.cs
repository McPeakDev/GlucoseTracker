using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static BCrypt.Net.BCrypt;

namespace GlucoseAPI.Models.Entities
{
    public class DoctorCreationBundle
    {
        [Required]
        public Doctor Doctor { get; set; }

        private string _password;

        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number")]
        [StringLength(128, MinimumLength = 8, ErrorMessage = "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number")]
        [Required(ErrorMessage = "Must be at least 8 characters long, 1 uppercase letter, 1 lowercase letter, 1 special character, and 1 number", AllowEmptyStrings = false)]
        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                if (!(value is null))
                {
                    _password = value;
                }
            }
        }
    }
}
