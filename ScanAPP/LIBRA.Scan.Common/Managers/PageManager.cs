using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class PageManager
    {
        public static long? pageId { get; set; }
        public static string? pageName { get; set; }
        public static string? pagePath { get; set; }
        public static long? pageOrder { get; set; }

        public static void SetPageId(long id)
        {
            pageId = id;
        }

        public static void SetPageOrder(long order)
        {
            pageOrder = order;
        }

        public static void SetPageName(string name)
        {
            pageName = name;
        }
    }
}
