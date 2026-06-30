using Avalonia.Controls;

namespace ZTCRM.UI.Views;

public partial class KvkkView : Window
{
    public KvkkView()
    {
        InitializeComponent();
    }

    private void CloseButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }
}