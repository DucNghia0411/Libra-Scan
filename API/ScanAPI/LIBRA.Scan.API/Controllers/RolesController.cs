using Azure.Core;
using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LIBRA.Scan.API.Controllers
{
    [Route("api/roles")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        IRolesService _rolesService;

        public RolesController
        (
            IRolesService rolesService
        ) 
        {
            this._rolesService = rolesService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<RequestResponse> Create(string roleName)
        {
            long result = await _rolesService.Create(roleName);

            if (result == (long)Code.InternalServerError)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = string.Empty,
                    Message = "Tạo Role thất bại!"
                };
            }

            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = roleName,
                Message = "Tạo Role thành công!"
            };
        }

        [HttpGet]
        [Route("getall")]
        public async Task<RequestResponse> GetAll()
        {
            try
            {
                IEnumerable<Role> roles = await _rolesService.GetAll();

                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = JsonConvert.SerializeObject(roles),
                    Message = "Lấy thông tin Roles thành công!"
                };
            }
            catch (Exception ex)
            {
                string errorDetail = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = errorDetail,
                    Message = "Lấy thông tin Roles thất bại!"
                };
            }
        }

        [HttpPost]
        [Route("addtorole")]
        public async Task<RequestResponse> AddUserToRole(long userId, long roleId)
        {
            try
            {
                long result = await _rolesService.AddUserToRole(userId, roleId);

                if (result == (long)Code.InternalServerError)
                {
                    return new RequestResponse()
                    {
                        StatusCode = Code.InternalServerError,
                        Content = string.Empty,
                        Message = "Tạo Role thất bại!"
                    };
                }

                if(result == (long)Code.NotFound)
                {
                    return new RequestResponse()
                    {
                        StatusCode = Code.NotFound,
                        Content = string.Empty,
                        Message = "User hoặc Role không tồn tại trong hệ thống!"
                    };
                }

                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = userId.ToString(),
                    Message = "Tạo Role thành công!"
                };
            }
            catch (Exception ex)
            {
                string errorDetail = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = errorDetail,
                    Message = "Thêm User vào Role thất bại!"
                };
            }
        }

        [HttpPost]
        [Route("getrolesbyuser")]
        public async Task<RequestResponse> GetRolesByUser(GetRolesByUserRequest request)
        {
            try
            {
                IList<string> roles = await _rolesService.GetRolesByUser(request.UserId);

                if (roles == null) 
                {
                    return new RequestResponse()
                    {
                        StatusCode = Code.NotFound,
                        Content = string.Empty,
                        Message = "User hoặc Role không tồn tại trong hệ thống!"
                    };
                }

                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = JsonConvert.SerializeObject(roles),
                    Message = "Lấy danh sách Roles thành công!"
                };
            }
            catch (Exception ex)
            {
                string errorDetail = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = errorDetail,
                    Message = "Lấy danh sách Roles thất bại!"
                };
            }
        }
    }
}
