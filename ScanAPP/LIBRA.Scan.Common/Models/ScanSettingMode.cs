using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Models
{
    public class ScanSettingMode
    {
        public int Id { get; set; }
        public string? Mode { get; set; }
    }

    public enum ScanMode
    {
        Default = 0,
        Custom = 1
    }
}
