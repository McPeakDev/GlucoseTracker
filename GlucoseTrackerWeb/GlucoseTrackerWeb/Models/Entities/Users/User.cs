using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    public abstract class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(150)]
        public string FirstName { get; set; }
        [StringLength(150)]
        public string MiddleName { get; set; }
        [Required(AllowEmptyStrings = false)]
        [StringLength(150)]
        public string LastName { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$", ErrorMessage = "Must match the form of example@example.com")]
        [Required(ErrorMessage = "Must match the form of example@example.com", AllowEmptyStrings = false)]
        [StringLength(255)]
        public string Email { get; set; }
        [RegularExpression(@"^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*$", ErrorMessage = "Must be 10 digits long")]
        [Required(ErrorMessage = "Must be 10 digits long", AllowEmptyStrings = false)]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Must be 10 digits long")]
        public string PhoneNumber { get; set; }
    }
}
