using Avalonia.Controls;
using ZTCRM.Models;
using Avalonia.Interactivity;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class OperatorView : Window
{
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
}
