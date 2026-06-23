namespace ZTCRM.Models;

public class OrgUnit
{
    public string? IsActiveText { get; set; }
    public int UnitId { get; set; }
    public string UnitName { get; set; }=string.Empty;
    public string UnitType { get; set; }=string.Empty;
    public int? ParentUnitId { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
}