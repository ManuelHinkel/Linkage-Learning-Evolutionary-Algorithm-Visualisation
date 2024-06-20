using Avalonia.Data;
using Avalonia.Data.Converters;
using DynamicData;
using System;
using System.Globalization;
using System.Linq;

namespace LLEAV.Converter
{
    /// <summary>
    /// Takes an enum and checks, if it its index is smaller or equal to another
    /// enum given by a string.
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class EnumOrSmallerEqualsStringConverter<TEnum> : IValueConverter
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

                int bindingIndex = Enum.GetValues<TEnum>().IndexOf(enumValue);
                int parameterIndex = enumParameters.Select(p => Enum.GetValues<TEnum>().IndexOf((TEnum)p)).Max();

                return bindingIndex <= parameterIndex;
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
