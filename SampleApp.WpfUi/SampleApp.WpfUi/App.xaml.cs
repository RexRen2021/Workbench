using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SampleApp.WpfUi.Helpers.App;
using SampleApp.WpfUi.Helpers.Extensions;
using SampleApp.WpfUi.Services;
using SampleApp.WpfUi.ViewModels.Pages;
using SampleApp.WpfUi.ViewModels.Windows;
using SampleApp.WpfUi.Views.Pages;
using SampleApp.WpfUi.Views.Windows;
using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;
using Wpf.Ui;
using Wpf.Ui.DependencyInjection;

namespace SampleApp.WpfUi;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    private IHost? _host;
    private ILogger<App>? _logger;

    public App()
    {
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }
        base.OnExit(e);
    }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var builder = Host.CreateApplicationBuilder(e.Args);
        builder.Environment.ContentRootPath = AppContext.BaseDirectory;

        builder.Services.AddSerilog((hostingContext, configuration) =>
        {
            configuration.ReadFrom.Configuration(builder.Configuration);
        });

        builder.Services.AddNavigationViewPageProvider();

        // App Host
        builder.Services.AddHostedService<ApplicationHostService>();

        // Main window container with navigation
        builder.Services.AddSingleton<MainWindow>();
        builder.Services.AddSingleton<MainWindowViewModel>();

        builder.Services.AddSingleton<INavigationService, NavigationService>();
        builder.Services.AddSingleton<ISnackbarService, SnackbarService>();
        builder.Services.AddSingleton<IContentDialogService, ContentDialogService>();

        builder.Services.AddSingleton<WindowsProviderService>();

        // AddSingleton pages
        builder.Services.AddSingleton<HomePage>();
        builder.Services.AddSingleton<HomeViewModel>();

        // All other pages and view models
        builder.Services.AddTransientFromNamespace("SampleApp.WpfUi.Views", AssemblyHelper.CurrentAsssembly);
        builder.Services.AddTransientFromNamespace("SampleApp.WpfUi.ViewModels", AssemblyHelper.CurrentAsssembly);

        _host = builder.Build();
        _logger= _host?.Services.GetRequiredService<ILogger<App>>();
        if (_host is not null)
        {
            await _host.StartAsync();
            _logger?.LogInformation("Application started successfully.");
        }
    }
}
