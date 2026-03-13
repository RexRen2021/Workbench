using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Logging;
using SampleApp.WpfUi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.WpfUi.ViewModels.Pages;

public partial class FirstViewModel: ViewModelBase
{
    [ObservableProperty]
    private int _randomNumber=0;

    [ObservableProperty]
    private string _sayHello = "Hello, World!";

    Timer _timer;
    private readonly ILogger<FirstViewModel> _logger;

    public FirstViewModel(ILogger<FirstViewModel> logger)
    {
        _timer = new Timer(OnTimerTick, null, Timeout.Infinite, 1000);
        _logger = logger;
    }

    public override void Dispose()
    {
        _timer.Dispose();
        base.Dispose();
    }

    public override Task OnNavigatedFromAsync()
    {
        _logger.LogInformation("Stop generating random number");
        _timer.Change(Timeout.Infinite, Timeout.Infinite);
        return base.OnNavigatedFromAsync();
    }

    public override Task OnNavigatedToAsync()
    {
        _logger.LogInformation("Start generating random number");
        _timer.Change(0, 1000);
        return base.OnNavigatedToAsync();
    }

    private void OnTimerTick(object? state)
    {
        RandomNumber = new Random().Next(0, 100);
        _logger.LogInformation("Generated random number: {RandomNumber}", RandomNumber);
    }
}
