using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;

namespace ExcelToDxfAvalonia.Converters
{
    public class EnumToDescriptionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            object attribute = value?
                .GetType()
                .GetField(value.ToString() ?? string.Empty)?
                .GetCustomAttributes(typeof(DescriptionAttribute), false)?
                .FirstOrDefault();

            if (attribute is DescriptionAttribute descriptionAttribute)
            {
                return descriptionAttribute.Description;
            }

            return value?.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
