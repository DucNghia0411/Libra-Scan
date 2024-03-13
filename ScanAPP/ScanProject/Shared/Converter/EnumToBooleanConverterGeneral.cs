using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Globalization;

namespace ScanProject.Shared.Converter
{
    /// <summary>
    /// Create by KhanhTL
    /// Ref at http://www.wpftutorial.net/RadioButton.html?showallcomments
    /// Need ConverterParamater when using
    /// </summary>
    public class EnumToBooleanConverterGeneral : IValueConverter
    {
        public object Convert(object value, Type targetType,
                               object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            string checkValue = value.ToString();
            string targetValue = parameter.ToString();
            bool bReturn = checkValue.Equals(targetValue,
                     StringComparison.InvariantCultureIgnoreCase);
            return bReturn;
        }

        public object ConvertBack(object value, Type targetType,
                                  object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return null;

            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue)
                return Enum.Parse(targetType, targetValue);

            return null;
        }

    }

}
