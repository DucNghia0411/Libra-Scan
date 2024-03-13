using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class Batch
{
    public long Id { get; set; }

    public string Name { get; set; } = null!;

    public long UserId { get; set; }

    public bool Deleted { get; set; }

    public string? Note { get; set; }

    public string Path { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual User User { get; set; } = null!;
}
