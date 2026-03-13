using SampleApp.WpfUi.Core;
using SampleApp.WpfUi.ViewModels.Pages;
using System;
using System.Collections.Generic;
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
using Wpf.Ui.Abstractions.Controls;
using Wpf.Ui.Controls;
using Button = Wpf.Ui.Controls.Button;

namespace SampleApp.WpfUi.Views.Pages;

/// <summary>
/// Interaction logic for HomePage.xaml
/// </summary>
public partial class HomePage : INavigableView<HomeViewModel>, IActionBar
{
    public HomePage(HomeViewModel viewModel)
    {
        InitializeComponent();

        ViewModel = viewModel;
        DataContext = this;
        ActionBar = CreateActionButtons();
    }

    public HomeViewModel ViewModel { get; }
    public FrameworkElement ActionBar { get; }

    private FrameworkElement CreateActionButtons()
    {
        var stackPanel = new System.Windows.Controls.StackPanel
        {
            Orientation = System.Windows.Controls.Orientation.Horizontal,
        };

        var addButton = new Button
        {
            Appearance = ControlAppearance.Primary,
            Icon = new SymbolIcon { Symbol = SymbolRegular.Add24 },
            Content = "测试用户消息",
            Width = 100,
            Margin = new Thickness(0, 0, 10, 0),
        };

        var exportButton = new Button
        {
            Content = "导出",
            Width = 100,
        };

        stackPanel.Children.Add(addButton);
        stackPanel.Children.Add(exportButton);

        return stackPanel;
    }
}
