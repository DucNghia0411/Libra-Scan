using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;

namespace ScanProject.Shared.Converter
{
    public class ZeroToNullConverter : IValueConverter
    {
        #region IValueConverter Members
        public String Zero
        {
            get;
            set;
        }
       

        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value.ToString() =="0")
            {
                return Zero;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        #endregion
    }

}
