using System.Text.Json;
using System.Text.Json.Serialization;

namespace CourseEnrollmentMVP.Services;

public class FileSentEmailStore : ISentEmailStore
{
    private readonly string _path;
    private readonly object _lock = new();

    public FileSentEmailStore(IWebHostEnvironment env)
    {
        // store next to content root
        _path = Path.Combine(env.ContentRootPath, "sent-emails.jsonl");
    }

    public Task LogAsync(SentEmailRecord record)
    {
        var line = JsonSerializer.Serialize(record, new JsonSerializerOptions { WriteIndented = false });
        lock (_lock)
        {
            File.AppendAllText(_path, line + Environment.NewLine);
        }
        return Task.CompletedTask;
    }

    public Task<List<SentEmailRecord>> GetAllAsync()
    {
        if (!File.Exists(_path)) return Task.FromResult(new List<SentEmailRecord>());
        var lines = File.ReadAllLines(_path);
        var list = new List<SentEmailRecord>();
        foreach (var l in lines)
        {
            try
            {
                var rec = JsonSerializer.Deserialize<SentEmailRecord>(l);
                if (rec != null) list.Add(rec);
            }
            catch { /* ignore malformed lines */ }
        }
        return Task.FromResult(list.OrderByDescending(r => r.Timestamp).ToList());
    }
}
