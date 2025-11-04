using System;
using System.Threading.Tasks;

namespace CourseEnrollmentMVP.Services;

public class ConsoleEmailSender : IEmailSender
{
    public Task SendEmailAsync(string to, string subject, string body)
    {
        // For development, just write to console. Replace with SMTP implementation for production.
        Console.WriteLine("--- Email sent (dev) ---");
        Console.WriteLine($"To: {to}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine(body);
        Console.WriteLine("------------------------");
        return Task.CompletedTask;
    }
}
