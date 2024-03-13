using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class Image
{
    public int Id { get; set; }

    public int? PageId { get; set; }

    public string Name { get; set; } = null!;

    public string? Path { get; set; }

    public int? NumberOrder { get; set; }

    public bool Deleted { get; set; }

    public string? Note { get; set; }

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual Page? Page { get; set; }
}
