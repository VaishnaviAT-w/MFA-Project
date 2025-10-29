using MFA.Core.Entities;

public class OtpMasterMap
{
    public OtpMaster ToEntity(OtpMasterDto dto)
    {
        return new OtpMaster
        {
            OtpId = dto.OtpId != Guid.Empty ? dto.OtpId : Guid.NewGuid(),
            UserId = dto.UserId,
            OtpCode = dto.OtpCode,
            OtpType = dto.OtpType,
            IsUsed = dto.IsUsed,
            ExpiresAt = dto.ExpiresAt,
            CreatedAt = dto.CreatedAt,
            IsActive = dto.IsActive
        };
    }

    public OtpMasterDto ToDto(OtpMaster entity)
    {
        return new OtpMasterDto
        {
            OtpId = entity.OtpId,
            UserId = entity.UserId,
            OtpCode = entity.OtpCode,
            OtpType = entity.OtpType,
            IsUsed = entity.IsUsed,
            ExpiresAt = entity.ExpiresAt,
            CreatedAt = entity.CreatedAt,
            IsActive = entity.IsActive
        };
    }
}
