using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class Page
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public long DocumentId { get; set; }

    public string? Path { get; set; }

    public int? NumberOrder { get; set; }

    public bool Deleted { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual ICollection<History> Histories { get; set; } = new List<History>();

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();
}
