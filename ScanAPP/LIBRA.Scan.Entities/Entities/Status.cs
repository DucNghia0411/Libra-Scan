using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class Status
{
    public long Id { get; set; }

    public string Status1 { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Document> DocumentDocProcessStatusNavigations { get; set; } = new List<Document>();

    public virtual ICollection<Document> DocumentDocScanStatusNavigations { get; set; } = new List<Document>();
}
