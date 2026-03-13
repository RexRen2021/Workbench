using Microsoft.Extensions.Logging;
using SampleApp.WpfUi.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.WpfUi.ViewModels.Pages;

public partial class SecondViewModel : ViewModelBase
{
    private readonly ILogger<SecondViewModel> _logger;
    public SecondViewModel(ILogger<SecondViewModel> logger)
    {
        _logger = logger;
    }

    public override Task OnNavigatedFromAsync()
    {
        _logger.LogInformation("Navigating from SecondViewModel");
        return base.OnNavigatedFromAsync();
    }
}
