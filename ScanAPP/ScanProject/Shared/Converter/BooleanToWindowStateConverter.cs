using System;
using System.Windows.Data;
using System.Globalization;
using Microsoft.Windows.Controls;

namespace ScanProject.Shared.Converter
{
    class BooleanToWindowStateConverter : IValueConverter
    {
        public WindowState FalseValue
        {
            get;
            set;
        }
        public WindowState TrueValue
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
