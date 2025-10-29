using MFA.Core.DTO;
using MFA.Core.Entities;
using MFA.Migrations;

namespace MFA.BI.Map
{
    public class ItUserMasterMap
    {
        public ITUserMaster AddMap(ItuserMasterDto dto, Guid createdBy)
        {
            return new ITUserMaster
            {
                UserId = Guid.NewGuid(),
                Name = dto.Name,
                Email = dto.Email,
                // Mobileno = dto.Mobileno,
                Password = dto.Password,
                //OTP = dto.OTP,
                OtpGeneratedOn = dto.OtpGeneratedOn.HasValue
                                 ? DateTime.SpecifyKind(dto.OtpGeneratedOn.Value, DateTimeKind.Utc)
                                 : null,
                IsVerified = dto.IsVerified,
                IsMfaEnabled = dto.IsMfaEnabled,
                MfaType = dto.MfaType,
                IsOtpLocked = dto.IsOtpLocked,
                OtpLockedOn = dto.OtpLockedOn,
                OtpAttempts = dto.OtpAttempts,
                CreatedBy = createdBy,
                CreatedOn = DateTime.UtcNow
            };
        }


        public ITUserMaster UpdateMap(ITUserMaster entity, ItuserMasterDto dto, Guid updatedBy)
        {
            entity.UserId = dto.UserId;
            entity.Name = dto.Name;
            entity.Email = dto.Email;
           // entity.Mobileno = dto.Mobileno;
            entity.Password = dto.Password;
          //  entity.OTP = dto.OTP;
            entity.OtpGeneratedOn = dto.OtpGeneratedOn.HasValue
                                   ? DateTime.SpecifyKind(dto.OtpGeneratedOn.Value, DateTimeKind.Utc)
                                   : null;
            entity.IsVerified = dto.IsVerified;
            entity.IsMfaEnabled = dto.IsMfaEnabled;
            entity.MfaType = dto.MfaType;
            entity.IsOtpLocked = dto.IsOtpLocked;
            entity.OtpLockedOn = dto.OtpLockedOn;
            entity.OtpAttempts = dto.OtpAttempts;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedOn = DateTime.UtcNow;

            return entity;
        }

        public ITUserMaster DeleteMap(ITUserMaster entity, Guid updatedBy)
        {
            entity.IsVerified = false;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedOn = DateTime.UtcNow;
            entity.OtpGeneratedOn = DateTime.UtcNow;

            return entity;
        }
    }
}
