using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace LIBRA.Scan.API.Controllers
{
    [Route("api/page")]
    [ApiController]
    public class PageController : ControllerBase
    {
        private readonly IPageService _pageService;

        public PageController
        (
            IPageService pageService
        )
        {
            _pageService = pageService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<RequestResponse> Create(PageCreateRequest request)
        {
            long pageId = await _pageService.Create(request);
            if (pageId == (long)Code.InternalServerError)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = string.Empty,
                    Message = "Tạo trang thất bại!"
                };
            }
            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = pageId.ToString(),
                Message = "Tạo trang thành công!"
            };
        }

        [HttpPost]
        [Route("getbydocumentid")]
        public async Task<RequestResponse> GetByDocumentID(int DocID)
        {
            try
            {
                IEnumerable<Page> pages = await _pageService.Get(a => a.DocumentId == DocID);
                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = JsonConvert.SerializeObject(pages),
                    Message = "Get all batches successfully!"
                };
            }
            catch (Exception ex)
            {
                string errorDetail = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = errorDetail,
                    Message = "Get document failed!"
                };
            }
        }
    }
}
