using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
namespace LIBRA.Scan.Entities.Requests
{
    public class ImageCreateRequest
    {
        public int? PageId { get; set; }
        public string Name { get; set; } = null!;
        public string? Path { get; set; }
        public string PageIcode { get; set; }
        public int? NumberOrder { get; set; }
        public bool Deleted { get; set; }
        public string? Note { get; set; }
        public IFormFile file { get; set; }
    }
}
