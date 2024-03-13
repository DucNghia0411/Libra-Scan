using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.API.Entities.Requests
{
    public class DocumentCreateRequest
    {
        public string Name { get; set; } = null!;

        public long BatchId { get; set; }

        public long UserId { get; set; }

        public string? AdministrativeDivision { get; set; }

        public long? DocumentNo { get; set; }

        public long? DocumentTypeId { get; set; }

        public long DocScanStatus { get; set; }

        public DateTime? ScannedDate { get; set; }

        public DateTime? RenderedDate { get; set; }

        public DateTime? SucceedDate { get; set; }

        public string? Note { get; set; }

        public bool? IsValid { get; set; }

        public long? UserQc { get; set; }

        public bool IsQc { get; set; }

        public DateTime? QcDate { get; set; }

        public string? Path { get; set; }

        public bool Deleted { get; set; }

        public long DocProcessStatus { get; set; }

        public string? DocumentYear { get; set; }

        public long? ScannedImages { get; set; }
    }
}
