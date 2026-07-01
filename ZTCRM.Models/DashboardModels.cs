namespace ZTCRM.Models;


public class DashboardSummary
{
    public int OpenCount { get; set; }
    public int ClosedCount { get; set; }
    public int TodayCount { get; set; }
    public decimal AvgResolutionHours { get; set; }
}

public class StatusDistributionItem
{
    public string? StatusName{ get; set; }
    public int RequestCount{ get; set; }
}

public class CategoryDistributionItem
{
    public string? CategoryName{ get; set; }
    public int RequestCount{ get; set; }
}
public class ChannelDistributionItem
{
    public string? ChannelName{ get; set; }
    public int RequestCount{ get; set; }
}
public class DailyTrendItem
{
    public DateTime? RequestDate{ get; set; }
    public int RequestCount{ get; set; }
}
public class UnitResolutionTimeItem
{
    public string? UnitName{ get; set; }
    public decimal AvgResolutionHours{ get; set; }
}

