using System.Threading.Tasks;
using System.Collections.Generic;

namespace CourseEnrollmentMVP.Services;

public class SentEmailRecord
{
    public DateTime Timestamp { get; set; }
    public string To { get; set; } = "";
    public string Subject { get; set; } = "";
    public string Body { get; set; } = "";
}

public interface ISentEmailStore
{
    Task LogAsync(SentEmailRecord record);
    Task<List<SentEmailRecord>> GetAllAsync();
}
