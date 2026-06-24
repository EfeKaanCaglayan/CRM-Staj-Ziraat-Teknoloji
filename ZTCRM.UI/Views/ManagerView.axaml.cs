using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class ManagerView : Window
{
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
}