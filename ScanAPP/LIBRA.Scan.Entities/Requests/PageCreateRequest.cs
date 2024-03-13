using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class PageCreateRequest
    {
        public string Name { get; set; } = null!;
        public string Icode { get; set; }
        public long DocumentId { get; set; }
        public string? Path { get; set; }
        public int? NumberOrder { get; set; }
        public bool Deleted { get; set; }
    }
}
