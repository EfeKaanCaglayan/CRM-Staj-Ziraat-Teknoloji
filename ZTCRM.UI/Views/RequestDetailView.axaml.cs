using Avalonia.Controls;
using ZTCRM.Models;

namespace ZTCRM.UI.Views;

public partial class RequestDetailView : Window
{
    public RequestDetailView() { InitializeComponent(); }
    
    public RequestDetailView(ServiceRequest request)
    {
        InitializeComponent();
        DataContext = request;
    }
}