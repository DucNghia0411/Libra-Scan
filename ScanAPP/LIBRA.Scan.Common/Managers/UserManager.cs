using LIBRA.Scan.Entities.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Common.Managers
{
    public abstract partial class UserManager
    {
        public static long _userId { get; set; }

        public static void SetUserId(long userId)
        {
            _userId = userId;
        }

    }
}
