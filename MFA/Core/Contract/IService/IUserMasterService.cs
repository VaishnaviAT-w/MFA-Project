using MFA.Core.DTO;
using MFA.Enum;

namespace MFA.Core.Contract.IService
{
    public interface IUserMasterService
    {
        Task<UserMasterResponse> AddOrUpdateItUserMaster(ItuserMasterDto dto);
        Task<UserMasterResponse> GetAllItUserMaster(PaginationRequest request);
        Task<ItUserMasterResponse> DeleteItUserMaster(Guid id);
        Task<VerifyResponse> VerifyOtp(string email, string otp);
        Task<OtpResponse> GetOtp(string email);
        Task<OtpResponse> SaveOtpAsync(string email, string otp);
    }
}
        