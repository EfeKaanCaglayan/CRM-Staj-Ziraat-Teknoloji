using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class OperatorView : Window
{
    private bool _requestsAsc = true;
    private string _lastRequestsColumn = "";

    public OperatorView() { InitializeComponent(); }

    public OperatorView(Staff staff)
    {
        InitializeComponent();
        DataContext = new OperatorViewModel(staff);
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
        if (DataContext is not OperatorViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastRequestsColumn == header) _requestsAsc = !_requestsAsc;
        else { _requestsAsc = true; _lastRequestsColumn = header; }

        vm.Requests = (header switch
        {
            "No"      => _requestsAsc ? vm.Requests.OrderBy(r => r.RequestId) : vm.Requests.OrderByDescending(r => r.RequestId),
            "Müşteri" => _requestsAsc ? vm.Requests.OrderBy(r => r.CustomerName) : vm.Requests.OrderByDescending(r => r.CustomerName),
            "Tarih"   => _requestsAsc ? vm.Requests.OrderBy(r => r.CreatedAt) : vm.Requests.OrderByDescending(r => r.CreatedAt),
            _         => vm.Requests.OrderBy(r => r.RequestId)
        }).ToList();

        e.Handled = true;
    }
}