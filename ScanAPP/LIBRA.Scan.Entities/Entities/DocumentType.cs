using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class DocumentType
{
    public long Id { get; set; }

    public string Type { get; set; } = null!;

    public string? Note { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();
}
