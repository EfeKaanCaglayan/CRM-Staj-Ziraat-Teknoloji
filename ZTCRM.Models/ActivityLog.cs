namespace ZTCRM.Models;

public class ActivityLog
{
    public int LogId { get; set; }
    public int RequestId { get; set; }
    public string OldStatus { get; set; } = string.Empty;
    public string NewStatus { get; set; } = string.Empty;
    public DateTime ChangedAt { get; set; }
    public bool SystemGenerated { get; set; }
    public string? ChangedBy { get; set; }
}