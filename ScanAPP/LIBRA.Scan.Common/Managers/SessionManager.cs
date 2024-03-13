using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Manager
{
    public abstract partial class SessionManager
    {
        public static string Token { get; set; }

        public static void SetToken(string jwtToken)
        {
            Token = jwtToken;
        }
    }
}
