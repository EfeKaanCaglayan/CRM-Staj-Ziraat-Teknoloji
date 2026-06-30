using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class LoginView : Window
{
    private void KvkkText_Click(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        var kvkkWindow = new KvkkView();
        kvkkWindow.ShowDialog(this);
    }
    public LoginView()
    {
        InitializeComponent();

        var tcknBox = this.FindControl<TextBox>("TcknBox");
        if (tcknBox != null)
        {
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
        }

        this.Loaded += (_, _) =>
        {
            if (DataContext is LoginViewModel vm)
            {
                vm.LoginSuccessful += staff =>
                {
                    Window nextWindow = staff.RoleName switch
                    {
                        "Admin"    => new AdminView(),
                        "Operator" => new OperatorView(staff),
                        "Staff"    => new StaffView(staff),
                        "Manager"  => new ManagerView(staff),
                        _          => new OperatorView(staff)
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