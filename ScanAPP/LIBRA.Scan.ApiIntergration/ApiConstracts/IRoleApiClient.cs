using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.ApiConstracts
{
    public interface IRoleApiClient
    {
        Task<List<string>> GetRolesByUserAsync(GetRolesByUserRequest request);
    }
}
