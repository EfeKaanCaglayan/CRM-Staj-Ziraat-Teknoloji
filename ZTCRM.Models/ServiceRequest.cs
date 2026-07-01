namespace ZTCRM.Models;

public class ServiceRequest
{
    public string? RequestType { get; set; }
    public string? Description { get; set; }
    public string? Priority { get; set; }
    public string? CurrentStatus { get; set; }
    public string? CategoryName { get; set; }
    public string? StaffName { get; set; }
    public string? ResolutionNote { get; set; }
    public string? CustomerName { get; set; }
    public int RequestId       { get; set; }
    public int CustomerId      { get; set; }
  
    public DateTime CreatedAt  { get; set; }
    public DateTime? ResolvedAt { get; set; }
    public DateTime? ClosedAt  { get; set; }
    public string? OperatorName { get; set; }
}