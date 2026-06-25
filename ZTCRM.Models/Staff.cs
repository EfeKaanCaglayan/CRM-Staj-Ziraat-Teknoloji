namespace ZTCRM.Models;

public class Staff
{
    
    public string  UnitIsActiveText { get; set; } =string.Empty;
    public string? IsActiveText { get; set; }
    public string? UnitName { get; set; }
    public int StaffId { get; set; }
    public int RoleId { get; set; }
    public string FullName { get; set; }=string.Empty;
    public string Username { get; set; }=  string.Empty;
    public string PasswordHash { get; set; }=string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLoginAt { get; set; }
    
    public string RoleName { get; set; }=string.Empty;
    
}