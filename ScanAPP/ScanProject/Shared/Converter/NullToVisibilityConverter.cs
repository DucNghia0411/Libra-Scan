using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ScanProject.Shared.Converter
{
    public class NullToVisibilityConverter : IValueConverter
    {
        #region IValueConverter Members
        public Visibility Null
        {
            get;
            set;
        }
        public Visibility NotNull
        {
            get;
            set;
        }

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value == null ? Null : NotNull;
        }
        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();            
        }
        #endregion
    }

}
