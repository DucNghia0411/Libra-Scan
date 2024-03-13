using System;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ScanProject.Shared.Converter
{
    class BooleanToVisibilityConverter : IValueConverter
    {
        public Visibility FalseValue
        {
            get;
            set;
        }
        public Visibility TrueValue
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean)
            {
                return ((Boolean)value) ? TrueValue : FalseValue;
            }
            throw new ArgumentException("Value should be Boolean type.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }

  

}
