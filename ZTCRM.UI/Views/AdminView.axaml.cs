using Avalonia.Controls;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class AdminView : Window
{
    public AdminView()
    {
        InitializeComponent();
        DataContext = new AdminViewModel();
    }
}