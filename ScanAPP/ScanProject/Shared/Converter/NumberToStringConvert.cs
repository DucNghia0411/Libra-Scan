using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace ScanProject.Shared.Converter
{
    class NumberToStringConvert : IValueConverter
    {
        public int Convert2Space
        {
            get;
            set;
        }
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is int)
            {
                int num = (int)value;
                return num== Convert2Space ? string.Empty : num.ToString();
            }
            else
            {
                throw new ArgumentException("Value should be Int type.");    
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
