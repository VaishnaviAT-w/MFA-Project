using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

public class SendOtpEmailService 
{
    private readonly IConfiguration _config;

    public SendOtpEmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOtpAsync(string toEmail, string otp)
    {
        try
        {
            var settings = _config.GetSection("Smtp");

            string host = settings["Host"];
            int port = int.Parse(settings["Port"]);
            string username = settings["Username"];
            string password = settings["Password"];
            string fromEmail = settings["FromEmail"];
            string fromName = settings["FromName"];

            using (var smtp = new SmtpClient(host, port))
            {
                smtp.Credentials = new NetworkCredential(username, password);
                smtp.EnableSsl = true;

                var mail = new MailMessage()
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = "Your OTP Code",
                    Body = $"Your One-Time Password (OTP) is: {otp} .\nIt will expire in 5 minutes.\n\nAT CyberTron Security",
                    IsBodyHtml = false
                };

                mail.To.Add(toEmail);

                await smtp.SendMailAsync(mail);
            }
        }
        catch (SmtpException ex)
        {
            throw new Exception($"SMTP Error: {ex.Message} - {ex.StatusCode}");
        }
        catch (Exception ex)
        {
            throw new Exception($"General Error: {ex.Message}");
        }
    }
}
