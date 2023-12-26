using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ExcelToDxfAvalonia.Extensions;
using ExcelToDxfAvalonia.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace ExcelToDxfAvalonia;

public partial class App : Application
{
    private readonly Startup startup = new ();

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ConsoleManager.OpenConsole();
            ConsoleManager.HideConsole();
        }

        new ServiceCollection()
            .AddSingleton<UnhandledExceptionLogger>()
            .AddLogging(builder => builder
                .AddConsole()
                .AddNLog(this.startup.Configuration))
            .BuildServiceProvider()
            .GetRequiredService<UnhandledExceptionLogger>()
            .SetupExceptionLogging();

        if (this.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = this.startup
                .ConfigureServices(new ServiceCollection())
                .BuildServiceProvider()
                .GetRequiredService<MainWindow>();

            desktop.Exit += this.DesktopExit;
            desktop.ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DesktopExit(object sender, ControlledApplicationLifetimeExitEventArgs e)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            ConsoleManager.CloseConsole();
        }
    }
}
