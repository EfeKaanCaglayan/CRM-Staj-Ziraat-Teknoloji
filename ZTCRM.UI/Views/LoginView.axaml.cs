using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class LoginView : Window
{
    public LoginView()
    {
        InitializeComponent();

        var tcknBox = this.FindControl<TextBox>("TcknBox")!;
        tcknBox.AddHandler(
            InputElement.TextInputEvent,
            (sender, e) =>
            {
                if (e.Text?.Any(c => !char.IsDigit(c)) == true)
                {
                    e.Handled = true;
                    return;
                }
                if (tcknBox.Text?.Length >= 11)
                    e.Handled = true;
            },
            RoutingStrategies.Tunnel);

        this.DataContextChanged += (_, _) =>
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSuccessful += roleName =>
                {
                    Window nextWindow = roleName switch
                    {
                        "Admin"    => new AdminView(),
                        "Operator" => new OperatorView(),
                        "Staff"    => new StaffView(),
                        "Manager"  => new ManagerView(),
                        _          => new OperatorView()
                    };
                    nextWindow.Show();
                    this.Close();
                };

                vm.CustomerLoginSuccessful += customer =>
                {
                    new CustomerView(customer).Show();
                    this.Close();
                };
            }
        };
    }
}