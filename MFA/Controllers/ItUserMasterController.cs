using MFA.BI.Service;
using MFA.Core.Contract.IRepo;
using MFA.Core.Contract.IService;
using MFA.Core.DTO;
using MFA.Core.Entities;
using MFA.Core.Helpers;
using MFA.Data.Repo;
using MFA.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace MFA.BI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserMasterController : ControllerBase
    {
        private readonly IUserMasterService _userService;
        private readonly IItUserMasterRepo _userRepo;
        private readonly JwtTokenHelper _jwtHelper;
        //private readonly IOtpMasterRepo _otpRepo;
        private readonly SendOtpEmailService _emailService;


        public UserMasterController(IUserMasterService userService, IItUserMasterRepo userRepo, JwtTokenHelper jwtHelper, SendOtpEmailService emailService)
        {
            _userService = userService;
            _userRepo = userRepo;
            _jwtHelper = jwtHelper;
            _emailService = emailService;
            //    _otpRepo = otpRepo;
        }

        [HttpPost("AddOrUpdateUser")]
        public async Task<UserMasterResponse> AddOrUpdateUser([FromBody] ItuserMasterDto userDto)
        {
            return await _userService.AddOrUpdateItUserMaster(userDto);
        }

        [HttpPost("GetAllUsers")]
        public async Task<UserMasterResponse> GetAllUsers([FromBody] PaginationRequest request)
        {
            return await _userService.GetAllItUserMaster(request);
        }

        [HttpPost("DeleteUser")]
        public async Task<ItUserMasterResponse> DeleteUser(Guid id)
        {
            return await _userService.DeleteItUserMaster(id);
        }

        [HttpPost("GetOtp")]
        [AllowAnonymous]
        public async Task<OtpResponse> GetOtp([FromBody] OtpRequest request)
        {
            return await _userService.GetOtp(request.Email);
        }

        [HttpPost("VerifyOtp")]
        [AllowAnonymous]
        public async Task<VerifyResponse> VerifyOtp([FromBody] VerifyOtpRequest request)
        {
            return await _userService.VerifyOtp(request.Email, request.Otp);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userRepo.ValidateUser(request.Email, request.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            if (user.IsMfaEnabled)
            {
                var otpResponse = await _userService.GetOtp(request.Email);
                return Ok(new
                {
                    Message = "MFA enabled. OTP sent to registered email.",
                    Email = request.Email,
                    Otp = otpResponse.Otp
                });
            }

            return Ok(new { Message = "Login successful (MFA disabled)" });
        }

        [HttpPost("SendOtp")]
        [AllowAnonymous]
        public async Task<OtpResponse> SendOtp([FromBody] string email)
        {
            var response = new OtpResponse();

            if (string.IsNullOrEmpty(email))
            {
                response.IsSent = false;
                response.Message = "Email is required.";
                return response;
            }

            var otp = new Random().Next(100000, 999999).ToString();

            try
            {
                await _emailService.SendOtpAsync(email, otp);

                var saveResponse = await _userService.SaveOtpAsync(email, otp);

                response.Email = email;
                response.Otp = otp;
                response.IsSent = true;
                response.Message = $"OTP sent & saved successfully to {email}";
                response.Result = ResponseModel.Success;
            }
            catch (Exception ex)
            {
                response.IsSent = false;
                response.Message = $"Failed to send OTP: {ex.Message}";
                response.Result = ResponseModel.Failed;
            }

            return response;
        }
    }
}







//        [HttpPost("SendOtp")]
//        [AllowAnonymous]
//        public async Task<OtpResponse> SendOtp([FromBody] string email)
//        {
//            var response = new OtpResponse();

//            if (string.IsNullOrEmpty(email))
//            {
//                response.IsSent = false;
//                response.Message = "Email is required.";
//                return response;
//            }

//            var otp = new Random().Next(100000, 999999).ToString();

//            try
//            {
//                var randomUserId = Guid.NewGuid();

//                var otpMaster = new OtpMaster
//                {
//                    OtpId = Guid.NewGuid(),
//                    UserId = Guid.NewGuid(),
//                    OtpCode = otp,
//                    OtpType = "EMAIL",
//                    IsUsed = false,
//                    ExpiresAt = DateTime.UtcNow.AddMinutes(5),
//                    CreatedAt = DateTime.UtcNow,
//                    IsActive = true
//                };

//                await _emailService.SendOtpAsync(email, otp);

//                response.Email = email;
//                response.Otp = otp;
//                response.IsSent = true;
//                response.Message = $"OTP sent successfully to {email}";
//                response.Result = ResponseModel.Success;
//            }
//            catch (Exception ex)
//            {
//                response.IsSent = false;
//                response.Message = $"Failed to send OTP: {ex.Message}";
//                response.Result = ResponseModel.Failed;
//            }

//            return response;
//        }
//    }
//}