using LIBRA.Scan.API.Entities.Entities;
using LIBRA.Scan.API.Entities.Requests;
using LIBRA.Scan.API.Entities.Respones;
using LIBRA.Scan.API.Service;
using LIBRA.Scan.API.Service.Constracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;


namespace LIBRA.Scan.API.Controllers
{
    [Route("api/document")]
    [ApiController]
    public class DocumentController : ControllerBase
    {
        private readonly IDocumentService _documentService;

        public DocumentController
        (
            IDocumentService documentService
        )
        {
            _documentService = documentService;
        }

        [HttpPost]
        [Route("create")]
        public async Task<RequestResponse> Create(DocumentCreateRequest request)
        {
            long documentId = await _documentService.Create(request);
            if (documentId == (long)Code.InternalServerError)
            {
                return new RequestResponse()
                {
                    StatusCode = Code.InternalServerError,
                    Content = string.Empty,
                    Message = "Tạo tài liệu thất bại!"
                };
            }
            return new RequestResponse()
            {
                StatusCode = Code.OK,
                Content = documentId.ToString(),
                Message = "Tạo tài liệu thành công!"
            };
        }

        [HttpPost]
        [Route("detail/{documentId}")]
        public async Task<IActionResult> GetById(long documentId)
        {
            var document = await _documentService.GetById(documentId);

            if (document == null) 
            {
                return BadRequest("Tài liệu không tồn tại!");
            }

            return Ok(document);
        }

        //[HttpPost]
        //[Route("getbybatchid")]
        //public async Task<RequestResponse> GetByBatchID(int BatchID)
        //{
        //    try
        //    {
        //        IEnumerable<Document> documents = await _documentService.Get(a=>a.BatchId == BatchID);
        //        return new RequestResponse()
        //        {
        //            StatusCode = Code.OK,
        //            Content = JsonConvert.SerializeObject(documents),
        //            Message = "Get all batches successfully!"
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorDetail = ex.InnerException != null ? ex.InnerException.ToString() : ex.Message.ToString();
        //        return new RequestResponse()
        //        {
        //            StatusCode = Code.InternalServerError,
        //            Content = errorDetail,
        //            Message = "Get document failed!"
        //        };
        //    }
        //}
    }
}
