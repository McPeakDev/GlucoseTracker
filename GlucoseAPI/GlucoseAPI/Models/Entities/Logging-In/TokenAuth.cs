///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//
//	Solution/Project:  GlucoseAPI/GlucoseAPI
//	File Name:         TokenAuth.cs
//	Description:       An Object to hold Token authentication Information for Users
//	Author:            Matthew McPeak, McPeakML@etsu.edu
//  Copyright:         Matthew McPeak, 2019
//  Team:              Sour Patch Kids
//
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GlucoseAPI.Models.Entities
{
    /// <summary>
    /// An Object to hold Token authentication Information for Users
    /// </summary>
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
