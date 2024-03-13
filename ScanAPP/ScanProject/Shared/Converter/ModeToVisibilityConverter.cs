using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace ScanProject.Shared.Converter
{
    public class ModeToVisibilityConverter : IValueConverter
    {
        public Visibility Central
        {
            get;
            set;
        }
        public Visibility Station
        {
            get;
            set;
        }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is int)
            {
                return (int)value == 1 ? Central : Station;
            }
            throw new ArgumentException("Value should be Boolean type.");
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
