using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using CourseEnrollmentMVP.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CourseEnrollmentMVP.Pages.Admin;

[Authorize(Roles = "Director")]
public class SentEmailsModel : PageModel
{
    private readonly ISentEmailStore _store;
    public List<SentEmailRecord> Emails { get; set; } = new();
    private readonly IEmailSender _email;
    public string? Message { get; set; }

    public SentEmailsModel(ISentEmailStore store, IEmailSender email)
    {
        _store = store;
        _email = email;
    }

    public async Task OnGetAsync()
    {
        Emails = await _store.GetAllAsync();
    }

    public async Task<IActionResult> OnPostAsync(string to, string subject, string body)
    {
        if (string.IsNullOrEmpty(to))
        {
            Message = "Please provide a recipient address.";
            Emails = await _store.GetAllAsync();
            return Page();
        }

        await _email.SendEmailAsync(to, subject, body);
        Message = $"Test email sent to {to} (logged).";
        Emails = await _store.GetAllAsync();
        return Page();
    }
}
