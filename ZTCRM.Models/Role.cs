namespace ZTCRM.Models;

public class Role
{
    public int RoleId{get;set;}
    public string RoleName{get;set;}=string.Empty;
    public string Description{get;set;} = string.Empty;
    public DateTime? CreatedAt {get;set;}
    
}