using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.Requests
{
    public class DocumentUpdateRequest
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long BatchId { get; set; }
        public string? AdministrativeDivision { get; set; }
        public long? DocumentNo { get; set; }
        public long? DocumentYear { get; set; }
        public long? DocumentTypeId { get; set; }
        public int? ScannedImages { get; set; }
        public string? Note { get; set; }
    }
}
