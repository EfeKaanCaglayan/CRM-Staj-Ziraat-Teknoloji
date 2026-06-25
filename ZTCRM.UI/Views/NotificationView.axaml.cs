using System.Collections.Generic;
using Avalonia.Controls;
using ZTCRM.Models;

namespace ZTCRM.UI.Views;

public partial class NotificationView : Window
{
    public NotificationView(List<Notification> notifications)
    {
        InitializeComponent();
        DataContext = notifications;
    }
    public NotificationView()
    {
        InitializeComponent();
    }
}