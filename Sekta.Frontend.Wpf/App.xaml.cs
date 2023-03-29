using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using ReactiveUI;
using Sekta.Core.IO;
using Sekta.Core.ModelView;
using Sekta.Frontend.Wpf.IO;
using Splat;

namespace Sekta.Frontend.Wpf;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public App()
    {
        Locator.CurrentMutable.RegisterViewsForViewModels(Assembly.GetCallingAssembly());
        Locator.CurrentMutable.RegisterViewsForViewModels(
            Assembly.GetAssembly(typeof(AppViewModel))
        );

        Locator.CurrentMutable.RegisterConstant<IOService>(new WpfIOService());
    }
}
