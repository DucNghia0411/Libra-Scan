using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class TokenCreateRequest
    {
        public string token { get; set; } = null!;
        public string created_date { get; set; } = null!;   
    }
}
