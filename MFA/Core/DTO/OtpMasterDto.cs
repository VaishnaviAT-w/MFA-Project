public class OtpMasterDto
{
    public Guid OtpId { get; set; }
    public Guid UserId { get; set; }
    public string OtpCode { get; set; } = string.Empty; 
    public string OtpType { get; set; } = "EMAIL"; 
    public bool IsUsed { get; set; } = false;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsActive { get; set; } = true;

}
