using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace SampleApp.WpfUi.Helpers.App;

public static class AssemblyHelper
{
    public static Assembly CurrentAsssembly => Assembly.GetExecutingAssembly();
}
