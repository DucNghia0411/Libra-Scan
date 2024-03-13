using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Common
{
    public static class ValueConvert
    {
        public static TModel ConvertToObject<TModel>(object sourceObject) where TModel : new()
        {
            TModel targetModel = new TModel();

            PropertyInfo[] sourceProperties = sourceObject.GetType().GetProperties();
            PropertyInfo[] targetProperties = typeof(TModel).GetProperties();

            foreach (PropertyInfo sourceProperty in sourceProperties)
            {
                PropertyInfo targetProperty = targetProperties.FirstOrDefault(p => p.Name == sourceProperty.Name);
                if (targetProperty != null && targetProperty.CanWrite)
                {
                    object value = sourceProperty.GetValue(sourceObject);
                    targetProperty.SetValue(targetModel, value);
                }
            }

            return targetModel;
        }

        public static string TruncateString(string data)
        {
            int maxLength = 20;
            if (data.Length > maxLength)
            {
                string truncatedData = data.Substring(0, maxLength) + "...";
                return truncatedData;
            }
            else
            {
                return data;
            }
        }
    }
}
