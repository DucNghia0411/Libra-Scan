using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class Role : IdentityRole<long>
{
    public override long Id { get; set; }

    public override string? Name { get; set; }

    public override string? NormalizedName { get; set; }

    public override string? ConcurrencyStamp { get; set; }

    public virtual ICollection<RoleClaim> RoleClaims { get; set; } = new List<RoleClaim>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
