using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class RoleClaim : IdentityRoleClaim<long>
{
    [Key]
    public override int Id { get; set; }

    public override long RoleId { get; set; }

    public override string? ClaimType { get; set; }

    public override string? ClaimValue { get; set; }

    public virtual Role Role { get; set; } = null!;
}
