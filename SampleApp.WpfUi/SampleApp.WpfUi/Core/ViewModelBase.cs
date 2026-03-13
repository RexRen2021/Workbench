using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Text;
using Wpf.Ui.Abstractions.Controls;

namespace SampleApp.WpfUi.Core;

public abstract partial class ViewModelBase : ObservableObject, INavigationAware, IDisposable
{
    public virtual void Dispose()
    {
    }

    public virtual Task OnNavigatedFromAsync()
    {
        return Task.CompletedTask;
    }

    public virtual Task OnNavigatedToAsync()
    {
        return Task.CompletedTask;
    }
}
