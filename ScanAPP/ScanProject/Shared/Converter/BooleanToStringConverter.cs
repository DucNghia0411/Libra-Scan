using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ScanProject.Shared.Converter
{
    class BooleanToStringConverter : IValueConverter
    {
        public string FalseValue //Invactive
        {
            get;
            set;
        }
        public string TrueValue // Active
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Boolean)
            {
                return ((Boolean)value) == true ? TrueValue : FalseValue;
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
