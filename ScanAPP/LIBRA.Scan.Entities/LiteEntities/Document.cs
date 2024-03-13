using System.Collections.Generic;

namespace LIBRA.Scan.Entities.LiteEntities
{
    public class Document
    {
        public long Id { get; set; } // integer
        public string Name { get; set; } = null!; // text(max)
        public long Batch_Id { get; set; } // integer
        public long User_Id { get; set; } // integer
        public string? Administrative_Division { get; set; } // text(max)
        public long? Document_No { get; set; } // integer
        public long? Document_Year { get; set; } // integer
        public long? Document_Type_Id { get; set; } // integer
        public long Doc_Scan_Status { get; set; } // integer
        public string? Scanned_Date { get; set; } // text(max)
        public string? Rendered_Date { get; set; } // text(max)
        public string? Succeed_Date { get; set; } // text(max)
        public long? Scanned_Images { get; set; } // integer
        public string? Note { get; set; } // text(max)
        public long? Is_Valid { get; set; } // integer
        public long? User_Qc { get; set; } // integer
        public long Is_Qc { get; set; } // integer
        public string? Qc_Date { get; set; } // text(max)
        public string? Path { get; set; } // text(max)
        public long Deleted { get; set; } // integer
        public long Doc_Process_Status { get; set; } // integer
        public string Icode { get; set; } = null!; // text(max)
        public virtual Batch Batch { get; set; } = null!;
        public virtual DocumentType DocumentType { get; set; } = null!;
        public virtual StatusProcess StatusProcess { get; set; } = null!;
        public virtual StatusScan StatusScan { get; set; } = null!;
        public virtual ICollection<History> Histories { get; set; } = new List<History>();
        public virtual ICollection<Page> Pages { get; set; } = new List<Page>();
    }
}
