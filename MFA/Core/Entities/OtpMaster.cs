using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MFA.Core.Entities
{
    [Table("OtpMaster")]
    public class OtpMaster
    {
        [Key]
        public Guid OtpId { get; set; } = Guid.NewGuid();

        public Guid UserId { get; set; }

       /* [ForeignKey("UserId")]
        public ITUserMaster? User { get; set; }*/

        [MaxLength(255)]
        public string OtpCode { get; set; } = string.Empty;

        [Required]
        public string OtpType { get; set; } = "EMAIL";

        public bool IsUsed { get; set; } = false;

        [Required]
        public DateTime ExpiresAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
