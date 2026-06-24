using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class StaffView : Window
{
    private void DetailButton_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button btn && btn.Tag is ServiceRequest request)
        {
            var dialog = new RequestDetailView(request);
            dialog.ShowDialog(this);
        }
    }

    public StaffView() { InitializeComponent(); }

    public StaffView(Staff staff)
    {
        InitializeComponent();
        DataContext = new StaffViewModel(staff);
    }
    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        var login = new LoginView();
        login.Show();
        this.Close();
    }
}