using Avalonia.Controls;
using ZTCRM.Models;
using ZTCRM.ViewModels;
using Avalonia.Interactivity;


namespace ZTCRM.UI.Views;

public partial class CustomerView : Window
{
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
}
