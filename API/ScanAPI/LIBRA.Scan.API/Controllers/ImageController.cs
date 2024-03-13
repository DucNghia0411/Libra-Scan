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
    [Route("api/image")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController
        (
            IImageService imageService
        )
        {
            _imageService = imageService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<RequestResponse> Create([FromForm] ImageCreateRequest request)
        {
            long imageId = await _imageService.Create(request);
            if (imageId == (long)Code.InternalServerError)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = string.Empty,
                    Message = "Tạo hình ảnh thất bại!"
                };
            }
            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = imageId.ToString(),
                Message = "Tạo hình ảnh thành công!"
            };
        }
        [HttpPost]
        [Route("getbypageid")]
        public async Task<RequestResponse> GetByPageID(int PageID)
        {
            try
            {
                IEnumerable<Image> image = await _imageService.Get(a => a.PageId == PageID);
                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = JsonConvert.SerializeObject(image),
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
