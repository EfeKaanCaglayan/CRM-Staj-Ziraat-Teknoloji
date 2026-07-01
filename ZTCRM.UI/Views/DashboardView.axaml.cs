using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class DashboardView : UserControl
{
    private readonly DashBoardViewModel _viewModel = new();

    public DashboardView()
    {
        InitializeComponent();
        DataContext = _viewModel;
        _viewModel.LoadDashboardData();
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs e)
    {
        _viewModel.LoadDashboardData();
    }
}