using System;
using Avalonia.Controls;
using ZTCRM.Models;
using ZTCRM.ViewModels;

namespace ZTCRM.UI.Views;

public partial class RequestDetailView : Window
{
    public RequestDetailView() { InitializeComponent(); }
    
    public RequestDetailView(ServiceRequest request)
    {
        InitializeComponent();
        Console.WriteLine($"DEBUG - RequestId: {request.RequestId}, CustomerId: {request.CustomerId}");
        DataContext = new RequestDetailViewModel(request);
    }
}