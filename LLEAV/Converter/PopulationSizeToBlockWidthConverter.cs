using Avalonia.Data.Converters;
using System;
using System.Globalization;

namespace LLEAV.Converter
{
    /// <summary>
    /// Takes the poulation size as an integer and returns the width of a population block.
    /// </summary>
    public class PopulationSizeToBlockWidthConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(int)) throw new ArgumentException("Illegal Converter Argument. Must be of type int");

            int valueAsInt = (int)value;

            return (int)(300 + (valueAsInt > 0 ? 20 * Math.Log2((int)value) : 0));
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
