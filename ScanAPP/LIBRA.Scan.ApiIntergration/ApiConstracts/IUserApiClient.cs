using LIBRA.Scan.Entities.Requests;
using LIBRA.Scan.Entities.Respones;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.ApiIntergration.ApiConstracts
{
    public interface IUserApiClient
    {
        Task<RequestResponse> Authenticate(AuthRequest request);
    }
}
