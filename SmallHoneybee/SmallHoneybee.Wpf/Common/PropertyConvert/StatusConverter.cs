using System;
using System.Windows.Data;

namespace SmallHoneybee.Wpf.Common.PropertyConvert
{
    [ValueConversion(typeof(sbyte), typeof(string))]
    public class StatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int id = int.Parse(value.ToString());
            if (id == 0)
            {
                return "#00CC00";
            }

            return "#FF0000";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
