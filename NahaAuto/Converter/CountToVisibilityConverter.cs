using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace NahaAuto.Converter
{
    public class CountToVisibilityConverter : IValueConverter
    {
        public CountToVisibilityConverter()
        {
            ValueIfTrue = Visibility.Collapsed;
            ValueIfNull = Visibility.Visible;
        }

        public Visibility ValueIfTrue { get; set; }

        public Visibility ValueIfNull { get; set; }

        public int CountIfTrue { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int))
                return ValueIfNull;

            return (int)value == CountIfTrue ? ValueIfTrue : VisibilityInversionConverter.InverseVisibility(ValueIfTrue);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("-remove 2-way");
        }
    }
}