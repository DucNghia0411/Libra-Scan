﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class UrlCreateRequest
    {
        public string scheme { get; set; } = null!;
        public string host_name { get; set; } = null!;
        public int port { get; set; }
        public int has_port { get; set; }
    }
}
