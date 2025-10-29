using MFA.Core.Entities;
using MFA.Migrations;
using System.Threading.Tasks;

namespace MFA.Core.Contract.IRepo
{
    public interface IItUserMasterRepo
    {
        Task<ITUserMaster> AddItUserMaster(ITUserMaster user);
        Task UpdateItUserMaster(ITUserMaster user);
        IQueryable<ITUserMaster> GetAllItUserMaster();
        Task<ITUserMaster?> ValidateUser(string email, string password);
        Task<ITUserMaster?> GetByEmail(string email);
        Task UpdateUser(ITUserMaster user);
        Task SaveOtpAsync(OtpMaster otp);
    }
}





