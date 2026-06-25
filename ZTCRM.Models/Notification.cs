namespace ZTCRM.Models;

public class Notification
{
    public int NotifId { get; set; }
    public int RequestId { get; set; }
    public int? StaffId { get; set; }
    public int? CustomerId { get; set; }
    public  string Channel { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsSent { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime?  SentAt { get; set; }
    
}