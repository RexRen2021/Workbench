using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SampleApp.WpfUi.Core;
using SampleApp.WpfUi.Helpers.Logging;
using SampleApp.WpfUi.ViewModels.Windows;
using SampleApp.WpfUi.Views.Pages;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Wpf.Ui;
using Wpf.Ui.Controls;
using Wpf.Ui.Extensions;
using MenuItem = Wpf.Ui.Controls.MenuItem;

namespace SampleApp.WpfUi.Views.Windows;


/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
    public MainWindowViewModel ViewModel { get; }

    private bool _isUserClosedPane;

    private bool _isPaneOpenedOrClosedFromCode;

    private object _currentPage;

    public List<object> NavigationItems { get; } =
        [
                new NavigationViewItem()
            {
                Content = "Home",
                ToolTip = "Home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
                TargetPageType = typeof(Views.Pages.HomePage),
            },
            new NavigationViewItem()
            {
                Content = "First",
                ToolTip = "First",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.FirstPage),
            },
            new NavigationViewItem()
            {
                Content = "Second",
                ToolTip = "Second",
                Icon = new SymbolIcon { Symbol = SymbolRegular.DataHistogram24 },
                TargetPageType = typeof(Views.Pages.SecondPage),
            },
        ];

    public List<object> NavigationFooter { get; } =
        [
            new NavigationViewItem()
            {
                Content = "Settings",
                ToolTip = "Settings",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Settings24 },
                TargetPageType = typeof(Views.Pages.SettingsPage),
            },
        ];

    public List<object> TrayMenuItems { get; } =
        [
            new MenuItem()
            {
                Header = "Home",
                Tag = "tray_home",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            },
            new Separator(),
            new MenuItem()
            {
                Header = "Close",
                Tag = "tray_close",
                Icon = new SymbolIcon { Symbol = SymbolRegular.Dismiss24 },
            },
        ];
    private readonly IConfiguration _configuration;
    private readonly ILogger<MainWindow>? _logger;
    private readonly IContentDialogService _contentDialogService;
    private readonly INavigationService _navigationService;

    private bool shutdown = false;

    public MainWindow(
        MainWindowViewModel viewModel,
        INavigationService navigationService,
        ISnackbarService snackbarService,
        IContentDialogService contentDialogService,
        IConfiguration configuration, ILogger<MainWindow>? logger = null
    )
    {
        _logger = logger;
        _configuration = configuration;

        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        snackbarService.SetSnackbarPresenter(SnackbarPresenter);
        navigationService.SetNavigationControl(NavigationView);
        contentDialogService.SetDialogHost(RootContentDialog);

        _contentDialogService = contentDialogService;
        _navigationService = navigationService;

        SetupTrayMenuEvents();
    }

    private void MainWindow_OnSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (_isUserClosedPane)
        {
            return;
        }

        _isPaneOpenedOrClosedFromCode = true;
        NavigationView.SetCurrentValue(NavigationView.IsPaneOpenProperty, e.NewSize.Width > 1200);
        _isPaneOpenedOrClosedFromCode = false;
    }

    private void NavigationView_OnPaneOpened(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = false;
    }

    private void NavigationView_OnPaneClosed(NavigationView sender, RoutedEventArgs args)
    {
        if (_isPaneOpenedOrClosedFromCode)
        {
            return;
        }

        _isUserClosedPane = true;
    }

    private void NavigationView_Navigated(NavigationView sender, NavigatedEventArgs args)
    {
        _currentPage = args.Page;

        if (_currentPage is IActionBar actionBarPage)
        {
            PageActionBarHost.Content = actionBarPage.ActionBar;
        }
        else
        {
            PageActionBarHost.Content = null;
        }
        _logger?.LogInformation($"Navigated to {_currentPage.GetType().Name}");
    }

    private async void ViewLog_Click(object sender, RoutedEventArgs e)
    {
        string? logFilePath = LogConfigurationHelper.GetFirstLogFilePath(_configuration);
        if (string.IsNullOrEmpty(logFilePath) || !File.Exists(logFilePath))
        {
            _logger?.LogWarning($"Log file not found or file does not exist:{logFilePath}");
            _ = System.Windows.MessageBox.Show("未找到日志文件路径。", "查看日志", System.Windows.MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        else
        {
            string Lexefn = "Notepad";
            try
            {
                await Task.Run(() =>
                {
                    Process.Start(Lexefn, logFilePath!);
                });
            }
            catch (Exception ex2)
            {
                _logger?.LogError($"Exec({Lexefn} {logFilePath}) err:{ex2.Message}");
            }
        }
    }

    private async void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        if (e.Cancel)
        {
            return;
        }
        if (this.shutdown)
        {
            //已经确认退出，清理主窗口内存、任务
            //NavigationView.Navigated -= OnNavigationViewNavigated;

        }
        else
        {
            e.Cancel = true;

            // We have to delay the execution through BeginInvoke to prevent potential re-entrancy
            await this.Dispatcher.BeginInvoke(new Action(async () => await this.ConfirmShutdown()));

        }
    }
    private async Task ConfirmShutdown()
    {
        var dialog = new ContentDialog()
        {
            Title = "是否退出?",
            Content = "确认要退出应用吗？",
            CloseButtonText = "取消",
            PrimaryButtonText = "确认",
            SecondaryButtonText = string.Empty,
            DefaultButton = ContentDialogButton.Primary
        };

        ContentDialogResult result = await _contentDialogService.ShowAsync(dialog, cancellationToken: default);
        this.shutdown = result == ContentDialogResult.Primary;

        if (this.shutdown)
        {
            Application.Current.Shutdown();
        }
    }

    private void SetupTrayMenuEvents()
    {
        foreach (var menuItem in TrayMenuItems)
        {
            if (menuItem is MenuItem item)
            {
                item.Click += OnTrayMenuItemClick;
            }
        }
    }

    private void OnTrayMenuItemClick(object sender, RoutedEventArgs e)
    {
        if (sender is not Wpf.Ui.Controls.MenuItem menuItem)
        {
            return;
        }

        var tag = menuItem.Tag?.ToString() ?? string.Empty;

        _logger?.LogInformation($"System Tray Click: {menuItem.Header}, Tag: {tag}");

        switch (tag)
        {
            case "tray_home":
                HandleTrayHomeClick();
                break;
            case "tray_close":
                HandleTrayCloseClick();
                break;
            default:
                if (!string.IsNullOrEmpty(tag))
                {
                    _logger?.LogInformation($"unknown Tag: {tag}");
                }

                break;
        }
    }

    private void HandleTrayHomeClick()
    {
        _logger?.LogInformation("Tray menu - Home Click");

        ShowAndActivateWindow();

        NavigateToPage(typeof(HomePage));
    }


    private void HandleTrayCloseClick()
    {
        _logger?.LogInformation("Tray menu - Close Click");
        ShowAndActivateWindow();
        Close();
    }

    private void ShowAndActivateWindow()
    {
        if (WindowState == WindowState.Minimized)
        {
            SetCurrentValue(WindowStateProperty, WindowState.Normal);
        }

        Show();
        _ = Activate();
        _ = Focus();
    }

    private void NavigateToPage(Type pageType)
    {
        try
        {
            NavigationView.Navigate(pageType);
        }
        catch (Exception ex)
        {
            _logger?.LogError($"NavigateToPage {pageType.Name} Error: {ex.Message}");
        }
    }

    private void NavigationView_Navigating(NavigationView sender, Wpf.Ui.Controls.NavigatingCancelEventArgs args)
    {
        _logger?.LogInformation($"Navigating happens!Source Page:{_currentPage?.GetType().FullName??"No Page Selected!"}, Target Page:{args.Page.GetType().FullName}");
    }
}