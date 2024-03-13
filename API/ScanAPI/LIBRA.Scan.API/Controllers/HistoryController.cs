using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LIBRA.Scan.API.Controllers
{
    [Route("api/history")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;

        public HistoryController
        (
            IHistoryService historyService
        )
        {
            _historyService = historyService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<RequestResponse> Create(HistoryCreateRequest request)
        {
            long historyId = await _historyService.Create(request);
            if (historyId == (long)Code.InternalServerError)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = string.Empty,
                    Message = "Tạo lịch sử hoạt động thất bại!"
                };
            }
            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = historyId.ToString(),
                Message = "Tạo lịch sử hoạt động thành công!"
            };
        }
    }
}
