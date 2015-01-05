using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace nmct.ba.cashlessproject.ui.Converters
{
    [ValueConversion(typeof(object), typeof(object))]
    public class SamenessConverter  : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return values.All(x => x == values[0]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibiliyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if ((bool)value == true) return Visibility.Visible;
            else return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
