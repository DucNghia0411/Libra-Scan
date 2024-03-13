using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class UpdateDeviceSettingRequest
    {
        public long id { get; set; }
        public string manufacturer { get; set; } = null!;
        public string device_name { get; set; } = null!;
        public string depth { get; set; } = null!;
        public int dpi { get; set; }
        public string size { get; set; } = null!;
        public int duplex { get; set; }
    }
}
