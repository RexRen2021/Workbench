using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SampleApp.WpfUi.Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace SampleApp.WpfUi.ViewModels.Pages;

public partial class SettingsViewModel : ViewModelBase
{
    private readonly ILogger<SettingsViewModel> _logger;
    public SettingsViewModel(ILogger<SettingsViewModel> logger)
    {
        _logger = logger;
    }

    //[RelayCommand]
    //private async Task TestUserMessageAsync()
    //{
    //    MessageBoxResult result = await MessageBox.QuestionAsync("This is a question and do you want to click OK?", "SH.MessageBoxCaptionQuestion");
    //    if (result == MessageBoxResult.Yes)
    //    {
    //        _= MessageBox.InformationAsync("You clicked OK!");
    //        _ = await MessageBox.WarningAsync("This is a warning message");
    //        _ = await MessageBox.ErrorAsync("This is a error message");
    //    }
    //    else
    //    {
    //        _ = MessageBox.InformationAsync("You didn't click OK!");
    //        _ = await MessageBox.WarningAsync("This is a warning message");
    //        _ = await MessageBox.ErrorAsync("This is a error message");
    //    }

    //    await Task.Delay(10000);
    //}
}
