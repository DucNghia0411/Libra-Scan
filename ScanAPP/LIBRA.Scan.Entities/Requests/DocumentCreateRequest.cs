using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class DocumentCreateRequest
    {
        public string Name { get; set; } = null!;
        public long BatchId { get; set; }
        public long UserId { get; set; }
        public string? AdministrativeDivision { get; set; }
        public long? DocumentNo { get; set; }
        public long? DocumentYear { get; set; }
        public long? DocumentTypeId { get; set; }
        public int? ScannedImages { get; set; }
        public string? Note { get; set; }
        public string? Path { get; set; }

    }
}
