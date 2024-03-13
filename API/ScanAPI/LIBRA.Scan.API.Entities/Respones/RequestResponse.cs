using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Entities.Respones
{
    public class RequestResponse
    {
        public Code StatusCode { get; set; }
        public string? Content { get; set; }
        public string? Message { get; set; }
    }
    public enum Code
    {
        OK = 200,
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        InternalServerError = 500,
    }
}
