using LIBRA.Scan.API.Common.Common;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using Microsoft.AspNetCore.Mvc;


namespace LIBRA.Scan.API.Service.Constracts
{
    public interface IUserService
    {
        Task<RequestResponse> Authencate([FromBody] AuthRequest request);
    }
}
