using LIBRA.Scan.Entities.LiteEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class UrlManager
    {
        public static string? Url { get; set; }

        public static void SetUrl(string url)
        {
            Url = url;
        }
    }
}
