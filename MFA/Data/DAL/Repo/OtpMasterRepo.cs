using MFA.Core.Entities;
using MFA.Data;
using Microsoft.EntityFrameworkCore;
using MFA.Core.Helpers; 

public class OtpMasterRepo : IOtpMasterRepo
{
    private readonly ApplicationDBContext _context;
    private readonly OtpEncryptionHelper _otpEncryptionHelper; 

    public OtpMasterRepo(ApplicationDBContext context, OtpEncryptionHelper otpEncryptionHelper)
    {
        _context = context;
        _otpEncryptionHelper = otpEncryptionHelper; 
    }

    public async Task<OtpMaster> AddOtp(OtpMaster otp)
    {
        await _context.Set<OtpMaster>().AddAsync(otp);
        await _context.SaveChangesAsync();
        return otp;
    }

    public async Task<OtpMaster?> GetLatestOtp(Guid userId)
    {
        return await _context.Otpmasters
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAt)  
            .FirstOrDefaultAsync();
    }

    public async Task UpdateOtp(OtpMaster otp)
    {
        _context.Set<OtpMaster>().Update(otp);
        await _context.SaveChangesAsync();
    }
}
