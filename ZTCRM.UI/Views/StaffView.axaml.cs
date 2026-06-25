using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class StaffView : Window
{
    private bool _poolAsc = true;
    private string _lastPoolColumn = "";
    private bool _myRequestsAsc = true;
    private string _lastMyRequestsColumn = "";

    public StaffView() { InitializeComponent(); }

    public StaffView(Staff staff)
    {
        InitializeComponent();
        DataContext = new StaffViewModel(staff);
    }
    private void NotificationButton_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is StaffViewModel vm)
        {
            vm.MarkNotificationsAsRead();
            var popup = new NotificationView(vm.StaffNotifications);
            popup.ShowDialog(this);
        }
    }

    private void DetailButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ServiceRequest request)
        {
            var dialog = new RequestDetailView(request);
            dialog.ShowDialog(this);
        }
    }

    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        var login = new LoginView();
        login.Show();
        this.Close();
    }

    private void PoolGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not StaffViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastPoolColumn == header) _poolAsc = !_poolAsc;
        else { _poolAsc = true; _lastPoolColumn = header; }

        vm.PoolRequests = (header switch
        {
            "No"       => _poolAsc ? vm.PoolRequests.OrderBy(r => r.RequestId) : vm.PoolRequests.OrderByDescending(r => r.RequestId),
            "Müşteri"  => _poolAsc ? vm.PoolRequests.OrderBy(r => r.CustomerName) : vm.PoolRequests.OrderByDescending(r => r.CustomerName),
            "Kategori" => _poolAsc ? vm.PoolRequests.OrderBy(r => r.CategoryName) : vm.PoolRequests.OrderByDescending(r => r.CategoryName),
            "Öncelik"  => _poolAsc ? vm.PoolRequests.OrderBy(r => r.Priority) : vm.PoolRequests.OrderByDescending(r => r.Priority),
            "Tarih"    => _poolAsc ? vm.PoolRequests.OrderBy(r => r.CreatedAt) : vm.PoolRequests.OrderByDescending(r => r.CreatedAt),
            _          => vm.PoolRequests.OrderBy(r => r.RequestId)
        }).ToList();

        e.Handled = true;
    }

    private void MyRequestsGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not StaffViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastMyRequestsColumn == header) _myRequestsAsc = !_myRequestsAsc;
        else { _myRequestsAsc = true; _lastMyRequestsColumn = header; }

        vm.MyRequests = (header switch
        {
            "No"       => _myRequestsAsc ? vm.MyRequests.OrderBy(r => r.RequestId) : vm.MyRequests.OrderByDescending(r => r.RequestId),
            "Müşteri"  => _myRequestsAsc ? vm.MyRequests.OrderBy(r => r.CustomerName) : vm.MyRequests.OrderByDescending(r => r.CustomerName),
            "Kategori" => _myRequestsAsc ? vm.MyRequests.OrderBy(r => r.CategoryName) : vm.MyRequests.OrderByDescending(r => r.CategoryName),
            "Öncelik"  => _myRequestsAsc ? vm.MyRequests.OrderBy(r => r.Priority) : vm.MyRequests.OrderByDescending(r => r.Priority),
            "Durum"    => _myRequestsAsc ? vm.MyRequests.OrderBy(r => r.CurrentStatus) : vm.MyRequests.OrderByDescending(r => r.CurrentStatus),
            _          => vm.MyRequests.OrderBy(r => r.RequestId)
        }).ToList();

        e.Handled = true;
    }
}