using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class UserToken : IdentityUserToken<long>
{
    public override long UserId { get; set; }

    public override string LoginProvider { get; set; } = null!;

    public override string Name { get; set; } = null!;

    public override string? Value { get; set; }

    public virtual User User { get; set; } = null!;
}
