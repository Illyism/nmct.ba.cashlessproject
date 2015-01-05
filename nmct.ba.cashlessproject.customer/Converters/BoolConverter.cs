using nmct.ba.cashlessproject.classlibrary;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace nmct.ba.cashlessproject.customer.Converters
{

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibiliyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if (string.IsNullOrEmpty(value as string)) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
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

    [ValueConversion(typeof(Customer), typeof(Visibility))]
    public class CustomerToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if ((value as Customer) == null) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(Employee), typeof(Visibility))]
    public class EmployeeToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            if ((value as Employee) == null) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
