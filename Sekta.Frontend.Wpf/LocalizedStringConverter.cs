using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Data;
using Sekta.Admx.Schema;
using Sekta.Core;

namespace Sekta.Frontend.Wpf;

public class LocalizedStringConverter : IValueConverter
{
    public PolicyDefinitionResources CurrentResources { get; set; }

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (CurrentResources == null || string.IsNullOrEmpty(value as string))
        {
            return value;
        }
        else
        {
            return ((string)value).LocalizeWith(CurrentResources);
        }
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
