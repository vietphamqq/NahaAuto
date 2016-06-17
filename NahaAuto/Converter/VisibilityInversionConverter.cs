using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NahaAuto.Converter
{
    public class VisibilityInversionConverter : IValueConverter
    {

        public static Visibility InverseVisibility(Visibility value)
        {
            return value == Visibility.Collapsed || value == Visibility.Hidden ? Visibility.Visible : Visibility.Collapsed;
        }
        public Visibility ValueIfTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
                throw new ArgumentNullException(nameof(value));

            return InverseVisibility((Visibility)value);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Visibility))
                throw new ArgumentNullException(nameof(value));

            return InverseVisibility((Visibility)value);
        }
    }
}