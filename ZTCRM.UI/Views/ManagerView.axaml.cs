using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class ManagerView : Window
{
    private bool _requestsAsc = true;
    private string _lastRequestsColumn = "";

    public ManagerView() { InitializeComponent(); }

    public ManagerView(Staff staff)
    {
        InitializeComponent();
        DataContext = new ManagerViewModel(staff);
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

    private void RequestsGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not ManagerViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastRequestsColumn == header) _requestsAsc = !_requestsAsc;
        else { _requestsAsc = true; _lastRequestsColumn = header; }

        vm.Requests = (header switch
        {
            "No"       => _requestsAsc ? vm.Requests.OrderBy(r => r.RequestId) : vm.Requests.OrderByDescending(r => r.RequestId),
            "Müşteri"  => _requestsAsc ? vm.Requests.OrderBy(r => r.CustomerName) : vm.Requests.OrderByDescending(r => r.CustomerName),
            "Personel" => _requestsAsc ? vm.Requests.OrderBy(r => r.StaffName) : vm.Requests.OrderByDescending(r => r.StaffName),
            "Kategori" => _requestsAsc ? vm.Requests.OrderBy(r => r.CategoryName) : vm.Requests.OrderByDescending(r => r.CategoryName),
            "Öncelik"  => _requestsAsc ? vm.Requests.OrderBy(r => r.Priority) : vm.Requests.OrderByDescending(r => r.Priority),
            "Durum"    => _requestsAsc ? vm.Requests.OrderBy(r => r.CurrentStatus) : vm.Requests.OrderByDescending(r => r.CurrentStatus),
            "Tarih"    => _requestsAsc ? vm.Requests.OrderBy(r => r.CreatedAt) : vm.Requests.OrderByDescending(r => r.CreatedAt),
            _          => vm.Requests.OrderBy(r => r.RequestId)
        }).ToList();

        e.Handled = true;
    }
}