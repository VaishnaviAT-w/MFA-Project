using MFA.Enum;

public class OtpResponse : ResultResponse
{
    public string Otp { get; set; } = string.Empty;
    public string Email { get; set; }
    public bool IsSent { get; set; }
    public string Message { get; set; }

}

public class VerifyResponse: ResultResponse
{
    public bool IsVerified { get; set; } = false;
    public string Message { get; set; } 
    public string? JwtToken { get; set; }
}
