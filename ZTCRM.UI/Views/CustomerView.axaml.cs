using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class CustomerView : Window
{
    private bool _requestsAsc = true;
    private string _lastRequestsColumn = "";

    public CustomerView() { InitializeComponent(); }

    public CustomerView(Customer customer)
    {
        InitializeComponent();
        var vm = new CustomerViewModel(customer);
        DataContext = vm;
    }

    private async void ShowDetailDialog(ServiceRequest request)
    {
        var dialog = new RequestDetailView(request);
        await dialog.ShowDialog(this);
    }

    private void DetailButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ServiceRequest request)
            ShowDetailDialog(request);
    }

    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        var login = new LoginView();
        login.Show();
        this.Close();
    }

    private void RequestsGrid_Sorting(object? sender, DataGridColumnEventArgs e)
    {
        if (DataContext is not CustomerViewModel vm) return;
        var header = e.Column.Header?.ToString() ?? "";
        if (_lastRequestsColumn == header) _requestsAsc = !_requestsAsc;
        else { _requestsAsc = true; _lastRequestsColumn = header; }

        vm.Requests = new ObservableCollection<ServiceRequest>(
            (header switch
            {
                "No"    => _requestsAsc ? vm.Requests.OrderBy(r => r.RequestId) : vm.Requests.OrderByDescending(r => r.RequestId),
                "Tür"   => _requestsAsc ? vm.Requests.OrderBy(r => r.RequestType) : vm.Requests.OrderByDescending(r => r.RequestType),
                "Durum" => _requestsAsc ? vm.Requests.OrderBy(r => r.CurrentStatus) : vm.Requests.OrderByDescending(r => r.CurrentStatus),
                "Tarih" => _requestsAsc ? vm.Requests.OrderBy(r => r.CreatedAt) : vm.Requests.OrderByDescending(r => r.CreatedAt),
                _       => vm.Requests.OrderBy(r => r.RequestId)
            }).ToList());

        e.Handled = true;
    }
}