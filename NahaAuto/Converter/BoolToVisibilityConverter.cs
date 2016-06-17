using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NahaAuto.Converter
{
    public class BoolToVisibilityConverter : IValueConverter
    {

        public BoolToVisibilityConverter()
        {
            ValueIfTrue = Visibility.Collapsed;
        }
        public Visibility ValueIfTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                throw new ArgumentNullException(nameof(value));

            return (bool)value ? ValueIfTrue : VisibilityInversionConverter.InverseVisibility(ValueIfTrue);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
                throw new ArgumentNullException(nameof(value));

            return (Visibility)value == ValueIfTrue;
        }
    }
}