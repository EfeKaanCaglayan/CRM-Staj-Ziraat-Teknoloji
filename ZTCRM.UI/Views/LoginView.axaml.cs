using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

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
    }
}