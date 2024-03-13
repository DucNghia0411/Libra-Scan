using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class ScanSettingManager
    {
        public static bool? _IsDuplex = false;
        public static bool? _IsDefault = true;
        public static object? _depth;
        public static object? _size;
        public static object? _dpi;
        public static int rescanIndex = 0;

        public static void  SetDuplex(bool isDuplex)
        {
            _IsDuplex = isDuplex;
        }

        public static void SetDepth(object depth)
        {
            _depth = depth;
        }

        public static void SetSize(object size) 
        {  
            _size = size; 
        }

        public static void SetDpi(object dpi) 
        {
            _dpi = dpi;
        }

        public static void UpdateRescanIndex()
        {
            if(rescanIndex > 1)
                rescanIndex = 0;
        }
    }
}
