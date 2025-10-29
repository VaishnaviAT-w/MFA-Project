using MFA.Enum;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json.Serialization;

namespace MFA.Core.DTO
{
    public class ItuserMasterDto
    {
        [Key]
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        //public string? Mobileno { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public bool IsMfaEnabled { get; set; } = false;
        public string? MfaType { get; set; }
        public int OtpAttempts { get; set; } = 0;
        public bool IsOtpLocked { get; set; } = false;
        public DateTime? OtpLockedOn { get; set; } = null;
        public DateTime? OtpGeneratedOn { get; set; }
        public bool IsVerified { get; set; } = false;
    }

    public class ItUserMasterResponse : ResultResponse
    {
        public Guid UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
       // public string? Mobileno { get; set; }
        public bool IsMfaEnabled { get; set; } = false;
        public string? MfaType { get; set; }
        public DateTime? OtpGeneratedOn { get; set; }
        public bool IsVerified { get; set; } = false;
        public int OtpAttempts { get; set; } = 0;
        public bool IsOtpLocked { get; set; } = false;
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedOn { get; set; } = DateTime.UtcNow;
    }

    public class UserMasterResponse : AdminSettingsPaginationResponse
    {
        public List<ItUserMasterResponse> UserResponse { get; set; } = new List<ItUserMasterResponse>();
        public Guid UserId { get; set; }
        public ResponseModel Result { get; set; }
    }

    public class VerifyOtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class OtpRequest
    {
        public string Email { get; set; } = string.Empty;

        //public MfaType MfaType { get; set; }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
       // public string Otp { get; set; } = string.Empty;

    }

}

