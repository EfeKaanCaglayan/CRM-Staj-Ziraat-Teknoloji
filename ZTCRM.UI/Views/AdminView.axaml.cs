using Avalonia.Controls;
using Avalonia.Interactivity;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class AdminView : Window
{
    public AdminView()
    {
        InitializeComponent();
        DataContext = new AdminViewModel();
    }
    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        var login = new LoginView();
        login.Show();
        this.Close();
    }
}