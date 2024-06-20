using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LLEAV.Converter
{
    /// <summary>
    /// 
    /// </summary>
    public class IntEqualsConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return (int)value == Int32.Parse((string)parameter);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
