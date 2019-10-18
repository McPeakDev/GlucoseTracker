using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GlucoseAPI.Models.Entities
{
    public class TokenAuth
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TokenId { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        [ForeignKey("Auth")]
        public int AuthId { get; set; }
        [Required]
        [StringLength(255)]
        public string Token { get; set; }
        public virtual User User { get; set; }
        public virtual Auth Auth { get; set; }

    }
}
