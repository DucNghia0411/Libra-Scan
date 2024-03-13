using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ScanProject.Shared.Converter
{
    class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                return System.Convert.ToDouble(value);
            }
            else
            {
                throw new ArgumentException("Value should be string type.");
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double)
            {
                return System.Convert.ToString(value);
            }
            else
            {
                throw new ArgumentException("Value should be double type.");
            }
        }

    }
}
