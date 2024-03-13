using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class UserLogin : IdentityUserLogin<long>
{
    public override string LoginProvider { get; set; } = null!;

    public override string ProviderKey { get; set; } = null!;

    public override string? ProviderDisplayName { get; set; }

    public override long UserId { get; set; }

    public virtual User User { get; set; } = null!;
}
