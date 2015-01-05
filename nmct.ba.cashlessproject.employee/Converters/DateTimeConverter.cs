using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace nmct.ba.cashlessproject.employee.Converters
{
    [ValueConversion(typeof(DateTime), typeof(String))]
    public class MyTimeAgoValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            var timeAgo = DateTime.UtcNow - (DateTime)value;
            if (timeAgo.TotalSeconds < 30)
            {
                return "just now";
            }

            if (timeAgo.TotalMinutes < 10)
            {
                return "a few minutes ago";
            }

            if (timeAgo.TotalMinutes < 60)
            {
                return "in the last hour";
            }

            if (timeAgo.TotalMinutes < 24 * 60)
            {
                return "in the last day";
            }

            return "previously";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(DateTime), typeof(String))]
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            DateTime time = (DateTime) value;
            return time.Day + "/" + time.Month + "/" + time.Year;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string time = value.ToString();
            string[] strtimes = time.Split('/');
            int[] times = new int[] { int.Parse(strtimes[0]), int.Parse(strtimes[1]), int.Parse(strtimes[2]) };
            return new DateTime(times[2], times[1], times[0]);
        }
    }

    [ValueConversion(typeof(DateTime), typeof(string))]
    public class TimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo cultureInfo)
        {
            DateTime time = (DateTime) value;
            return time.Hour + ":" + time.Minute;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
