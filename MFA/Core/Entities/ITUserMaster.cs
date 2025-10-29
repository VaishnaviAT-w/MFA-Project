using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MFA.Core.Entities
{
    [Table("ITUserMasters")]
    public class ITUserMaster
    {
        [Key]
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }

        // public string? Mobileno {  get; set; }
        public bool IsVerified { get; set; } = false;
        public DateTime? OtpGeneratedOn { get; set; } = DateTime.UtcNow;
        public bool IsMfaEnabled { get; set; } = false;
        public string? MfaType { get; set; }
        public int OtpAttempts { get; set; } = 0;
        public bool IsOtpLocked { get; set; } = false;
        public DateTime? OtpLockedOn { get; set; } = null;
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;

       // public ICollection<OtpMaster> Otps { get; set; } = new List<OtpMaster>();

    }
}




