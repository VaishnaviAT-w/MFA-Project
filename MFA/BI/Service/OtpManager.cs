//using MFA.Core.Contract.IRepo;
//using MFA.Core.DTO;
//using MFA.Enum;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Threading.Tasks;

//namespace MFA.BI.Service
//{
//    public class OtpManager
//    {
//        private readonly IItUserMasterRepo _userRepo;

//        public OtpManager(IItUserMasterRepo userRepo)
//        {
//            _userRepo = userRepo;
//        }

//        public async Task<OtpResponse> GetOtp(string email)
//        {
//            var response = new OtpResponse();

//            var user = await _userRepo.GetAllItUserMaster()
//                .FirstOrDefaultAsync(u => u.Email == email);

//            if (user == null)
//            {
//                response.Result = ResponseModel.NotFound;
//                return response;
//            }

//            // 🔒 Check if OTP is locked
//            if (user.IsOtpLocked)
//            {
//                response.Result = ResponseModel.Failed;
//                response.Otp = null;
//                Console.WriteLine($"[LOCKED] User {email} cannot generate OTP anymore.");
//                return response;
//            }

//            // 🔢 Generate new OTP
//            var random = new Random();
//            string otp = random.Next(100000, 999999).ToString();

//            user.OTP = otp;
//            user.OtpGeneratedOn = DateTime.UtcNow;
//            user.OtpAttempts = 0; // reset attempts on new OTP
//            user.IsOtpLocked = false;

//            await _userRepo.UpdateItUserMaster(user);

//            Console.WriteLine($"[EMAIL OTP] sent to {user.Email}: {otp}");

//            response.Otp = otp;
//            response.Result = ResponseModel.Success;
//            return response;
//        }


//        public async Task<VerifyResponse> VerifyOtp(string email, string otp)
//        {
//            var response = new VerifyResponse();

//            var user = await _userRepo.GetAllItUserMaster()
//                .FirstOrDefaultAsync(u => u.Email == email);

//            if (user == null)
//            {
//                response.Result = ResponseModel.NotFound;
//                response.IsVerified = false;
//                return response;
//            }

//            if (user.IsOtpLocked)
//            {
//                response.Result = ResponseModel.Failed;
//                response.IsVerified = false;
//                Console.WriteLine($"[LOCKED] OTP disabled for {email}");
//                return response;
//            }

//            if (user.OtpGeneratedOn.HasValue &&
//               (DateTime.UtcNow - user.OtpGeneratedOn.Value).TotalMinutes > 5)
//            {
//                response.Result = ResponseModel.Failed;
//                response.IsVerified = false;
//                Console.WriteLine($"[EXPIRED] OTP expired for {email}");
//                return response;
//            }

//            if (user.OTP == otp)
//            {
//                user.IsVerified = true;
//                user.OtpAttempts = 0;
//                await _userRepo.UpdateItUserMaster(user);

//                var jwtToken = GenerateJwtToken(user); 

//                response.IsVerified = true;
//                response.Result = ResponseModel.Success;
//                response.JwtToken = jwtToken;
//                Console.WriteLine($"[SUCCESS] OTP verified for {email}");
//                return response;
//            }

//            user.OtpAttempts++;
//            Console.WriteLine($"[FAILED ATTEMPT] {email} - Attempt {user.OtpAttempts}");

//            if (user.OtpAttempts >= 3)
//            {
//                user.IsOtpLocked = true;
//                Console.WriteLine($"[LOCKED] {email} exceeded max OTP attempts");
//            }

//            await _userRepo.UpdateItUserMaster(user);

//            response.Result = ResponseModel.Failed;
//            response.IsVerified = false;
//            return response;
//        }
//    }
//}