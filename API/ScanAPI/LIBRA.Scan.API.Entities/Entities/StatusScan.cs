using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class StatusScan
{
    public long Id { get; set; }

    public string Status { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
