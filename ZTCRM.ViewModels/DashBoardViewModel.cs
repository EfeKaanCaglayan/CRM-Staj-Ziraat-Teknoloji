using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using ZTCRM.Data;
using ZTCRM.Models;

namespace ZTCRM.ViewModels;

public partial class DashBoardViewModel : ObservableObject
{
    private readonly DashboardRepository _dashboardRepository = new();

    private static readonly SKColor[] Palette =
    {
        SKColor.Parse("#C8102E"), // kurumsal kırmızı
        SKColor.Parse("#0369A1"), // mavi
        SKColor.Parse("#15803D"), // yeşil
        SKColor.Parse("#D97706"), // turuncu
        SKColor.Parse("#7C3AED"), // mor
        SKColor.Parse("#0E7490"), // camgöbeği
        SKColor.Parse("#BE185D"), // pembe
        SKColor.Parse("#4B5563"), // gri
        SKColor.Parse("#65A30D"), // limon yeşili
        SKColor.Parse("#B45309"), // kahve
    };

    [ObservableProperty] private DashboardSummary _summary = new();
    [ObservableProperty] private bool _isLoading = false;

    public ObservableCollection<CategoryDistributionItem> CategoryDistribution { get; } = new();
    public ObservableCollection<StatusDistributionItem> StatusDistribution { get; } = new();
    public ObservableCollection<ChannelDistributionItem> ChannelDistribution { get; } = new();
    public ObservableCollection<DailyTrendItem> DailyTrend { get; } = new();
    public ObservableCollection<UnitResolutionTimeItem> UnitResolutionTime { get; } = new();

    [ObservableProperty] private ISeries[] _statusSeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] _channelSeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] _categorySeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] _dailyTrendSeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] _unitResolutionSeries = Array.Empty<ISeries>();

    [ObservableProperty] private Axis[] _categoryXAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] _dailyTrendXAxes = Array.Empty<Axis>();
    [ObservableProperty] private Axis[] _unitXAxes = Array.Empty<Axis>();

    public void LoadDashboardData()
    {
        IsLoading = true;

        Summary = _dashboardRepository.GetSummary();

        CategoryDistribution.Clear();
        foreach (var item in _dashboardRepository.GetCategorySummary()) CategoryDistribution.Add(item);

        StatusDistribution.Clear();
        foreach (var item in _dashboardRepository.GetStatusSummary()) StatusDistribution.Add(item);

        ChannelDistribution.Clear();
        foreach (var item in _dashboardRepository.GetChannelSummary()) ChannelDistribution.Add(item);

        DailyTrend.Clear();
        foreach (var item in _dashboardRepository.GetDailyTrend()) DailyTrend.Add(item);

        UnitResolutionTime.Clear();
        foreach (var item in _dashboardRepository.GetUnitResolutionTime()) UnitResolutionTime.Add(item);

        BuildCharts();
        IsLoading = false;
    }

    private void BuildCharts()
    {
        // Durum dağılımı - Pasta grafik
        StatusSeries = StatusDistribution.Select((item, i) => new PieSeries<int>
        {
            Values = new[] { item.RequestCount },
            Name = item.StatusName,
            Fill = new SolidColorPaint(Palette[i % Palette.Length]),
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 12,
            DataLabelsFormatter = _ => item.RequestCount.ToString()
        }).ToArray();

        // Kanal dağılımı - Pasta grafik
        ChannelSeries = ChannelDistribution.Select((item, i) => new PieSeries<int>
        {
            Values = new[] { item.RequestCount },
            Name = item.ChannelName,
            Fill = new SolidColorPaint(Palette[i % Palette.Length]),
            DataLabelsPaint = new SolidColorPaint(SKColors.White),
            DataLabelsSize = 12,
            DataLabelsFormatter = _ => item.RequestCount.ToString()
        }).ToArray();

        // Kategori dağılımı - Bar grafik
        CategorySeries = new ISeries[]
        {
            new ColumnSeries<int>
            {
                Values = CategoryDistribution.Select(c => c.RequestCount).ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#C8102E")),
                Name = "Başvuru Sayısı",
                MaxBarWidth = 40
            }
        };
        CategoryXAxes = new[]
        {
            new Axis
            {
                Labels = CategoryDistribution.Select(c => c.CategoryName ?? "—").ToArray(),
                LabelsRotation = 20,
                TextSize = 11
            }
        };

        // Zaman içinde trend - Çizgi grafik
        DailyTrendSeries = new ISeries[]
        {
            new LineSeries<int>
            {
                Values = DailyTrend.Select(d => d.RequestCount).ToArray(),
                Stroke = new SolidColorPaint(SKColor.Parse("#C8102E"), 3),
                Fill = new SolidColorPaint(SKColor.Parse("#C8102E").WithAlpha(30)),
                GeometrySize = 6,
                GeometryStroke = new SolidColorPaint(SKColor.Parse("#C8102E"), 2),
                GeometryFill = new SolidColorPaint(SKColors.White),
                Name = "Başvuru"
            }
        };
        DailyTrendXAxes = new[]
        {
            new Axis
            {
                Labels = DailyTrend.Select(d => d.RequestDate?.ToString("dd.MM") ?? "").ToArray(),
                LabelsRotation = 45,
                TextSize = 10
            }
        };

        // Birim bazında ortalama çözüm süresi - Bar grafik
        UnitResolutionSeries = new ISeries[]
        {
            new ColumnSeries<decimal>
            {
                Values = UnitResolutionTime.Select(u => u.AvgResolutionHours).ToArray(),
                Fill = new SolidColorPaint(SKColor.Parse("#0369A1")),
                Name = "Ort. Süre (Saat)",
                MaxBarWidth = 45
            }
        };
        UnitXAxes = new[]
        {
            new Axis
            {
                Labels = UnitResolutionTime.Select(u => u.UnitName ?? "—").ToArray(),
                LabelsRotation = 15,
                TextSize = 11
            }
        };
    }
}