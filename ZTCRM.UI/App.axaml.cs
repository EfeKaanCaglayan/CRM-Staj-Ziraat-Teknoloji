using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ZTCRM.UI.ViewModels;
using ZTCRM.UI.Views;
using System;
using ZTCRM.ViewModels;

namespace ZTCRM.UI;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            try
            {
                desktop.MainWindow = new LoginView
                {
                    DataContext = new LoginViewModel(),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"HATA: {ex}");
                throw;
            }
        }

        base.OnFrameworkInitializationCompleted();
    }
}