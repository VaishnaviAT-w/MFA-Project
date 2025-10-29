using MFA.Core.Entities;

public interface IOtpMasterRepo
{
    Task<OtpMaster> AddOtp(OtpMaster otp);
    Task UpdateOtp(OtpMaster otp);

   // IQueryable<OtpMaster> GetAllOtp();
    Task<OtpMaster?> GetLatestOtp(Guid userId);
}
