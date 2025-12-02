using MFA.BI.Map;
using MFA.Core.Contract.IRepo;
using MFA.Core.Contract.IService;
using MFA.Core.DTO;
using MFA.Core.Entities;
using MFA.Core.Helpers;
using MFA.Enum;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace MFA.BI.Service
{
    public class ITUserMasterService : IUserMasterService
    {
        private readonly IItUserMasterRepo _userRepo;
        private readonly ItUserMasterMap _mapper;
        private readonly JwtTokenHelper _jwtHelper;
        private readonly IOtpMasterRepo _otpRepo;
        private readonly OtpEncryptionHelper _otpEncryptionHelper;
        private readonly int _otpLockDurationHours;
        private readonly int _otpExpiryMinutes;
        private readonly int _maxOtpAttempts;
      //  private readonly SendOtpEmailService _emailService;

        public ITUserMasterService(
            IItUserMasterRepo userRepo,
            ItUserMasterMap mapper,
            JwtTokenHelper jwtHelper,
            IConfiguration configuration,
            IOtpMasterRepo otpRepo,
            OtpEncryptionHelper otpEncryptionHelper)
            //SendOtpEmailService emailService)
        {
            _userRepo = userRepo;
            _mapper = mapper;
            _jwtHelper = jwtHelper;
            _otpRepo = otpRepo;
            _otpEncryptionHelper = otpEncryptionHelper;
            //_emailService = emailService;

            _otpLockDurationHours = configuration.GetValue<int>("OtpSettings:LockDurationHours");
            _otpExpiryMinutes = configuration.GetValue<int>("OtpSettings:OtpExpiryMinutes");
            _maxOtpAttempts = configuration.GetValue<int>("OtpSettings:MaxAttempts");
        }

        public bool IsBase64String(string s)
        {
            if (string.IsNullOrEmpty(s)) return false;

            Span<byte> buffer = new Span<byte>(new byte[s.Length]);
            return (s.Length % 4 == 0) && Convert.TryFromBase64String(s, buffer, out _);
        }

        public async Task<UserMasterResponse> AddOrUpdateItUserMaster(ItuserMasterDto ituserMasterDto)
        {
            var response = new UserMasterResponse();
            try
            {
                var entity = await _userRepo
                    .GetAllItUserMaster()
                    .FirstOrDefaultAsync(x => x.Name == ituserMasterDto.Name);

                if (entity == null)
                {
                    var createdBy = Guid.NewGuid();
                    entity = _mapper.AddMap(ituserMasterDto, createdBy);

                    var createdUser = await _userRepo.AddItUserMaster(entity);
                    response.UserId = createdUser.UserId;
                    response.Result = createdUser.UserId != Guid.Empty ? ResponseModel.Success : ResponseModel.Failed;
                }
                else
                {
                    var updatedBy = Guid.NewGuid();
                    entity = _mapper.UpdateMap(entity, ituserMasterDto, updatedBy);
                    await _userRepo.UpdateItUserMaster(entity);
                    response.Result = ResponseModel.Success;
                }
            }
            catch (Exception)
            {
                response.Result = ResponseModel.Failed;
            }
            return response;
        }

        public async Task<UserMasterResponse> GetAllItUserMaster(PaginationRequest request)
        {
            var response = new UserMasterResponse();
            try
            {
                var query = _userRepo.GetAllItUserMaster();

                query = query.Where(x => x.IsVerified == true);

                response.TotalCount = await query.CountAsync();
                response.Index = request.Index;
                response.PageSize = request.PageSize;
                response.PageCount = (int)Math.Ceiling(response.TotalCount / (double)request.PageSize);

                response.UserResponse = await query
                    .Skip(request.PageSize * (request.Index - 1))
                    .Take(request.PageSize)
                    .Select(x => new ItUserMasterResponse
                    {
                        UserId = x.UserId,
                        Name = x.Name,
                        //Mobileno = x.Mobileno,
                        Email = x.Email,
                        Password = x.Password,
                        //OTP = x.OTP,
                        IsMfaEnabled = x.IsMfaEnabled,
                        MfaType = x.MfaType,
                        IsOtpLocked = x.IsOtpLocked,
                        OtpAttempts = x.OtpAttempts,
                        OtpGeneratedOn = x.OtpGeneratedOn,
                        IsVerified = x.IsVerified,
                        CreatedBy = x.CreatedBy,
                        UpdatedBy = x.UpdatedBy,    
                        CreatedOn = x.CreatedOn,
                        UpdatedOn = x.UpdatedOn
                    })
                    .ToListAsync();

                response.Result = response.UserResponse.Count > 0
                    ? ResponseModel.Success
                    : ResponseModel.Failed;
            }
            catch (Exception)
            {
                response.Result = ResponseModel.Failed;
                response.UserResponse = new List<ItUserMasterResponse>();
            }
            return response;
        }

        public async Task<ItUserMasterResponse> DeleteItUserMaster(Guid id)
        {
            var response = new ItUserMasterResponse();
            try
            {
                var entity = await _userRepo.GetAllItUserMaster()
                    .FirstOrDefaultAsync(x => x.UserId == id);

                if (entity == null)
                {
                    response.Result = ResponseModel.NotFound;
                    return response;
                }

                if (entity.IsVerified == false)
                {
                    response.Result = ResponseModel.AlreadyExists;
                    return response;
                }

                var updatedBy = Guid.NewGuid(); 
                entity = _mapper.DeleteMap(entity, updatedBy);

                await _userRepo.UpdateItUserMaster(entity);
                response.Result = ResponseModel.Success;
            }
            catch (Exception)
            {
                response.Result = ResponseModel.Failed;
            }
            return response;
        }

        public async Task<OtpResponse> GetOtp(string email)
        {
            var response = new OtpResponse();

            var user = await _userRepo.GetAllItUserMaster()
                                      .FirstOrDefaultAsync(u => u.Email!.ToLower() == email.ToLower());

            if (user == null)
            {
                response.Result = ResponseModel.NotFound;
                return response;
            }

            if (user.IsOtpLocked && user.OtpLockedOn.HasValue &&
                (DateTime.UtcNow - user.OtpLockedOn.Value).TotalHours >= _otpLockDurationHours)
            {
                user.IsOtpLocked = false;
                user.OtpAttempts = 0;
                user.OtpLockedOn = null;
                await _userRepo.UpdateItUserMaster(user);
            }

            if (user.IsOtpLocked)
            {
                response.Result = ResponseModel.Failed;
                return response;
            }

            var otpValue = RandomNumberGenerator.GetInt32(100000, 1000000).ToString("D7");
            var encryptedOtp = _otpEncryptionHelper.Encrypt(otpValue);

            var existingOtp = await _otpRepo.GetLatestOtp(user.UserId);

            if (existingOtp != null)
            {
                existingOtp.OtpCode = encryptedOtp;
                existingOtp.OtpType = "Email";
                existingOtp.ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes);
                existingOtp.CreatedAt = DateTime.UtcNow;
                existingOtp.IsUsed = false;

                await _otpRepo.UpdateOtp(existingOtp);
            }
            else
            {
                var otpMaster = new OtpMaster
                {
                    UserId = user.UserId,
                    OtpCode = encryptedOtp,
                    OtpType = "EMAIL",
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes),
                    CreatedAt = DateTime.UtcNow,
                    IsUsed = false
                };

                await _otpRepo.AddOtp(otpMaster);
            }

            Console.WriteLine($"[OTP GENERATED] User: {email}, OTP: {otpValue}");

            response.Otp = otpValue;
            response.Result = ResponseModel.Success;
            return response;
        }

        public async Task<VerifyResponse> VerifyOtp(string email, string otp)
        {
            var response = new VerifyResponse();

            var user = await _userRepo.GetAllItUserMaster()
                                      .FirstOrDefaultAsync(u => u.Email != null && u.Email.ToLower() == email.ToLower());

            if (user == null)
            {
                response.Result = ResponseModel.NotFound;
                response.IsVerified = false;
                response.Message = "User not found.";
                return response;
            }

            if (user.IsOtpLocked && user.OtpLockedOn.HasValue &&
                (DateTime.UtcNow - user.OtpLockedOn.Value).TotalHours < _otpLockDurationHours)
            {
                response.Result = ResponseModel.Failed;
                response.IsVerified = false;
                response.Message = "OTP attempts exceeded. User is temporarily locked.";
                return response;
            }
            else if (user.IsOtpLocked)
            {
                user.IsOtpLocked = false;
                user.OtpAttempts = 0;
                user.OtpLockedOn = null;
                await _userRepo.UpdateItUserMaster(user);
            }

            var latestOtp = await _otpRepo.GetLatestOtp(user.UserId);
            if (latestOtp == null)
            {
                response.Result = ResponseModel.Failed;
                response.IsVerified = false;
                response.Message = "No OTP found. Please generate OTP first.";
                return response;
            }

            var otpExpiryTime = latestOtp.ExpiresAt.ToUniversalTime();
            if (latestOtp.IsUsed || otpExpiryTime < DateTime.UtcNow)
            {
                response.Result = ResponseModel.Failed;
                response.IsVerified = false;
                response.Message = "OTP expired or already used.";
                return response;
            }

            var decryptedOtp = _otpEncryptionHelper.Decrypt(latestOtp.OtpCode)?.Trim();

            if (decryptedOtp == otp?.Trim())
            {
                latestOtp.IsUsed = true;
                await _otpRepo.UpdateOtp(latestOtp);

                user.IsVerified = true;
                user.OtpAttempts = 0;
                user.IsOtpLocked = false;
                user.OtpLockedOn = null;
                user.UpdatedOn = DateTime.UtcNow;
                await _userRepo.UpdateItUserMaster(user);

                var token = _jwtHelper.CreateJwtToken(user);

                response.Result = ResponseModel.Success;
                response.IsVerified = true;
                response.JwtToken = token;
                response.Message = "OTP verified successfully.";

                Console.WriteLine($"[OTP VERIFIED SUCCESS] User: {email}, OTP: {decryptedOtp}");
            }
            else
            {
                user.OtpAttempts++;
                if (user.OtpAttempts >= _maxOtpAttempts)
                {   
                    user.IsOtpLocked = true;
                    user.OtpLockedOn = DateTime.UtcNow;
                }

                user.UpdatedOn = DateTime.UtcNow;
                await _userRepo.UpdateItUserMaster(user);

                response.Result = ResponseModel.Failed;
                response.IsVerified = false;
                response.JwtToken = null;
                response.Message = "OTP is invalid. Please try again.";

                Console.WriteLine($"[OTP FAILED] User: {email}, Input OTP: {otp}, Attempts: {user.OtpAttempts}");
            }
            return response;
        }

        public async Task<OtpResponse> SaveOtpAsync(string email, string otp)
        {
            var response = new OtpResponse();

            try
            {
                var user = await _userRepo.GetByEmail(email);
                if (user == null)
                {
                    response.IsSent = false;
                    response.Message = "User not found.";
                    response.Result = ResponseModel.NotFound;
                    return response;
                }

                var encryptedOtp = _otpEncryptionHelper.Encrypt(otp);

                var otpMaster = new OtpMaster
                {
                    OtpId = Guid.NewGuid(),
                    UserId = user.UserId,
                    OtpCode = encryptedOtp,
                    OtpType = "EMAIL",
                    IsUsed = false,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(_otpExpiryMinutes),
                    CreatedAt = DateTime.UtcNow,
                    IsActive = true
                };

                await _userRepo.SaveOtpAsync(otpMaster);

                response.Email = email;
                response.Otp = otp;
                response.IsSent = true;
                response.Message = $"OTP saved successfully for {email}";
                response.Result = ResponseModel.Success;
            }
            catch (Exception ex)
            {
                response.IsSent = false;
                response.Message = $"Error saving OTP: {ex.Message}";
                response.Result = ResponseModel.Failed;
            }

            return response;
        }
    }
}


