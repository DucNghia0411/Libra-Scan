using LIBRA.Scan.API.Common.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Entities.Requests
{
    public class UserAssignRequest
    {
        public long Id { get; set; }
        public List<RoleSelectedItem> Users { get; set; } = new List<RoleSelectedItem>();
    }
}
