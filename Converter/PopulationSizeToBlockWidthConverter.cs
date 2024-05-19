using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LLEAV.Converter
{
    public class PopulationSizeToBlockWidthConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value.GetType() != typeof(int)) throw new ArgumentException("Illegal Converter Argument. Must be of type int");
            
            int valueAsInt = (int)value;

            return 300 + (valueAsInt > 0 ? 20 * Math.Log((int) value) : 0);
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
