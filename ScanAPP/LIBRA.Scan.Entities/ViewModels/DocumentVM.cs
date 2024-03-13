using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LIBRA.Scan.Entities.ViewModels
{
    public class DocumentVM
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public long? BatchId { get; set; }
        public long? UserId { get; set; }
        public string? AdministrativeDivision { get; set; }
        public long? DocumentNo { get; set; }
        public long? DocumentYear { get; set; }
        public string? DocumentType { get; set; } 
        public long? DocumentTypeId { get; set; }
        public string? DocScanStatus { get; set; } 
        public string? ScannedDate { get; set; } 
        public string? RenderedDate { get; set; } 
        public string? SucceedDate { get; set; }
        public long? ScannedImages { get; set; } 
        public string? Note { get; set; }
        public string? Path { get; set; } 
        public string? DocProcessStatus { get; set; } 
        public string? Icode { get; set; } 
    }
}
