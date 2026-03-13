using Microsoft.Extensions.Logging;
using SampleApp.WpfUi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.WpfUi.ViewModels.Pages;

public partial class HomeViewModel : ViewModelBase
{
    private readonly ILogger<HomeViewModel> _logger;
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
}
