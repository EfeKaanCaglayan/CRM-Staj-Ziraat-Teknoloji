using Avalonia.Controls;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class CustomerView : Window
{
    public CustomerView() { InitializeComponent(); }
    
    public CustomerView(Customer customer)
    {
        InitializeComponent();
        DataContext = new CustomerViewModel(customer);
    }
}