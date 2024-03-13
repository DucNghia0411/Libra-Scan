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
    [Route("api/batch")]
    [ApiController]
    public class BatchController : ControllerBase
    {
        private readonly ILogger<BatchController> _logger;
        private readonly IBatchService _batchService;

        public BatchController
        (
            ILogger<BatchController> logger
            , IBatchService batchService
        )
        {
            _logger = logger;
            _batchService = batchService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<RequestResponse> Create(BatchCreateRequest request)
        {
            long batchId = await _batchService.Create(request);
            if (batchId == (long)Code.InternalServerError)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = string.Empty,
                    Message = "Tạo Batch thất bại!"
                };
            }
            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = batchId.ToString(),
                Message = "Tạo Batch thành công!"
            };
        }

        [HttpGet]
        [Route("getall")]
        public async Task<RequestResponse> GetAll()
        {
            try
            {
                IEnumerable<Batch> batches = await _batchService.GetAll();
                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = JsonConvert.SerializeObject(batches),
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
                    Message = "Get all batches failed!"
                };
            }
        }

        [HttpGet]
        [Route("detail/{batchName}")]
        public async Task<IActionResult> GetByName(string batchName)
        {
            var batch = await _batchService.GetByName(batchName);

            if (batch == null)
            {
                return BadRequest("Batch không tồn tại!");
            }

            return Ok(batch);
        }


        [HttpPost]
        [Route("getbyid")]
        public async Task<RequestResponse> GetByID(long Id)
        {
            try
            {
                IEnumerable<Batch> batches = await _batchService.Get(a=>a.Id == Id);
                return new RequestResponse()
                {
                    StatusCode = Code.OK,
                    Content = JsonConvert.SerializeObject(batches.FirstOrDefault()),
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
                    Message = "Get all batches failed!"
                };
            }
        }
    }
}
