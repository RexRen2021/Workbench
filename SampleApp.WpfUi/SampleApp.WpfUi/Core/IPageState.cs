using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApp.WpfUi.Core;

public interface IPageState
{
    bool CanNavigateFrom { get; }
    bool CanNavigateTo { get; }
    bool CanClose { get; }
}
