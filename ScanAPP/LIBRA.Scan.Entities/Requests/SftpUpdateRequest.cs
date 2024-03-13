using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class SftpUpdateRequest
    {
        public long id { get; set; }
        public string host_name { get; set; } = null!;
        public int port_number { get; set; }
        public string user_name { get; set; } = null!;
        public string password { get; set; } = null!;
        public string host_key { get; set; } = null!;
        public string folder_path { get; set; } = null!;
    }
}
