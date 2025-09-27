using Microsoft.AspNetCore.Identity.UI.Services;

namespace CustomerSubscriptions.Web.Services;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        // Stub: In production, plug in SMTP/SendGrid/etc.
        Console.WriteLine($"EMAIL to {email} | {subject}\n{htmlMessage}");
        return Task.CompletedTask;
    }
}

