namespace ZTCRM.Models;

public class Category
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int? DefaultUnitId { get; set; }
    public int? ParentCategoryId { get; set; }
}