using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ScanProject.Shared.Converter
{
    class StringToVisibilityConverter : IValueConverter
    {
        public string Visible
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Collapsed;
            else if (value != null && value is string)
            {
                if (value.ToString() == Visible)
                    return Visibility.Visible;
                return Visibility.Collapsed;
            }
            else
            {
                throw new ArgumentException("Value should be Boolean type.");
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
