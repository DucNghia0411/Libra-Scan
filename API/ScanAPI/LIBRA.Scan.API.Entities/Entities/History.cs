using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class History
{
    public long Id { get; set; }

    public long? DocumentId { get; set; }

    public int? PageId { get; set; }

    public string? Note { get; set; }

    public string Actions { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public long UserId { get; set; }

    public int? ImageId { get; set; }

    public virtual Document? Document { get; set; }

    public virtual Image? Image { get; set; }

    public virtual Page? Page { get; set; }
}
