namespace ZTCRM.Models;

public class ServiceRequest
{
    public string? StaffName { get; set; }
    public string? ResolutionNote { get; set; }
    public string? CustomerName { get; set; }
    public int RequestId       { get; set; }
    public int CustomerId      { get; set; }
    public string RequestType  { get; set; } = string.Empty;
    public string Description  { get; set; } = string.Empty;
    public string? CategoryName { get; set; }
    public string Priority     { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;
    public DateTime CreatedAt  { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt  { get; set; }
    public string? OperatorName { get; set; }
}