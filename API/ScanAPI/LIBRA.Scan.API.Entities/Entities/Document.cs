using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class Document
{
    public long Id { get; set; }

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

    public virtual Batch Batch { get; set; } = null!;

    public virtual StatusProcess DocProcessStatusNavigation { get; set; } = null!;

    public virtual StatusScan DocScanStatusNavigation { get; set; } = null!;

    public virtual DocumentType? DocumentType { get; set; }

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Page> Pages { get; set; } = new List<Page>();

    public virtual User User { get; set; } = null!;
}
