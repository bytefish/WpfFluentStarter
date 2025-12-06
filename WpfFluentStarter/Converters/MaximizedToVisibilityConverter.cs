// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfFluentStarter.Converters;

public class MaximizedToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is WindowState state)
        {
            // Show the Maximize button if the state is Normal or Minimized
            return (state == WindowState.Normal || state == WindowState.Minimized)
                   ? Visibility.Visible
                   : Visibility.Collapsed;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
