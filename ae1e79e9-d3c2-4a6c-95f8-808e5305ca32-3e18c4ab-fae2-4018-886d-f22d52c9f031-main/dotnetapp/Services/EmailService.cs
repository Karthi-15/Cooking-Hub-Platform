
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendDecisionEmail(string toEmail, string status)
    {
        var subject = "Cooking Class Request Update";
        var body = $"Your cooking class request has been {status.ToLower()}. Please check the website for more details.";

        using var client = new SmtpClient(_config["Email:SmtpServer"])
        {
            Port = int.Parse(_config["Email:Port"]),
            Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
            EnableSsl = true
        };

        var mail = new MailMessage(_config["Email:From"], toEmail, subject, body);
        await client.SendMailAsync(mail);
    }
    public async Task SendFeedbackEmail(string toEmail, string username, string feedbackText)
{
    var subject = "Thank You for Your Feedback";
    var body = $"Hi {username},\n\nThank you for your feedback:\n\"{feedbackText}\"\n\nWe appreciate your input.\n\nBest regards,\nAdmin Team";

    using var client = new SmtpClient(_config["Email:SmtpServer"])
    {
        Port = int.Parse(_config["Email:Port"]),
        Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
        EnableSsl = true
    };

    var mail = new MailMessage(_config["Email:From"], toEmail, subject, body);
    await client.SendMailAsync(mail);
}

}
