using System;
using System.Windows.Data;
using SmallHoneybee.Common;

namespace SmallHoneybee.Wpf.Common.PropertyConvert
{
    [ValueConversion(typeof(sbyte), typeof(bool))]
    public class POStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return byte.Parse(value.ToString()) != (byte)DataType.POStatusCategory.Completed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
