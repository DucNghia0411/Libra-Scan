using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class UserClaim : IdentityUserClaim<long>
{
    public override int Id { get; set; }

    public override long UserId { get; set; }

    public override string? ClaimType { get; set; }

    public override string? ClaimValue { get; set; }

    public virtual User User { get; set; } = null!;
}
