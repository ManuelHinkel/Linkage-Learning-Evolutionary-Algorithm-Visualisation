using Avalonia.Data.Converters;
using Avalonia.Data;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DynamicData;

namespace LLEAV.Converter
{
    public class EnumEqualsStringConverter<TEnum> : IValueConverter
    where TEnum : struct, Enum
    {
        public virtual object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            var isEnum = value != null &&
               value is TEnum;
            if (isEnum &&
                parameter != null &&
                Enum.TryParse<TEnum>(value.ToString(), out var enumValue))
            {
                var enumParameters = parameter.ToString().Trim('\'').Split(',').Select(str =>
                {
                    if (Enum.TryParse<TEnum>(str, out var enumParameter))
                    {
                        return (TEnum?)enumParameter;
                    }

                    return null;
                });
                return enumParameters.Any(enumParameter => enumValue.Equals(enumParameter));
            }

            // converter used for the wrong type
            return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            return new BindingNotification(new NotSupportedException(), BindingErrorType.Error);
        }
    }
}
