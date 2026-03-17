using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using SampleApp.WpfUi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.WpfUi.ViewModels.Pages;

public partial class HomeViewModel : ViewModelBase
{
    private readonly ILogger<HomeViewModel> _logger;

    [ObservableProperty]
    private int _counter = 0;

    [ObservableProperty]
    private Boolean _isBusy = false;

    public HomeViewModel(ILogger<HomeViewModel> logger)
    {
        _logger= logger;
        _logger.LogInformation("HomeViewModel created");
    }

    public override Task OnNavigatedToAsync()
    {
        _logger.LogInformation("Navigated to HomeViewModel");
        return base.OnNavigatedToAsync();
    }

    [RelayCommand]
    private Task AddAsync()
    {
        Counter++;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task DoingAsync()
    {
        IsBusy = true;
        await Task.Delay(10000);
        IsBusy = false;
    }
}
