using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using ScanProject;

namespace ScanProject.Shared.Converter
{
    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DetailViewStatus)
            {
                if (String.Compare((string)parameter, "ReadOnly") == 0)
                    return ((DetailViewStatus)value) == DetailViewStatus.Adding ? true : false;
                if (String.Compare((string)parameter, "CreateNew") == 0)
                    return ((DetailViewStatus)value) == DetailViewStatus.Adding ? false : true;
                else
                    return ((DetailViewStatus)value) == DetailViewStatus.None ? false : true;
            }
            else
            {
                throw new ArgumentException("Value should be DetailViewStatus type.");
            }
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }
}
