using LIBRA.Scan.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Services.Constracts
{
    public interface IAuthService
    {
        Task<long> Create(TokenCreateRequest request);
        string GetLatestToken();
    }
}
