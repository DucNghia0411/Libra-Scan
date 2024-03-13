using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace LIBRA.Scan.API.Entities.Entities;

public partial class User : IdentityUser<long>
{
    public override long Id { get; set; }

    public string? FullName { get; set; }

    public override string? UserName { get; set; }

    public override string? NormalizedUserName { get; set; }

    public override string? Email { get; set; }

    public override string? NormalizedEmail { get; set; }

    public override bool EmailConfirmed { get; set; }

    public override string? PasswordHash { get; set; }

    public override string? SecurityStamp { get; set; }

    public override string? ConcurrencyStamp { get; set; }

    public override string? PhoneNumber { get; set; }

    public override bool PhoneNumberConfirmed { get; set; }

    public override bool TwoFactorEnabled { get; set; }

    public DateTime? LockoutEnd { get; set; }

    public override bool LockoutEnabled { get; set; }

    public override int AccessFailedCount { get; set; }

    public virtual ICollection<Batch> Batches { get; set; } = new List<Batch>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<UserClaim> UserClaims { get; set; } = new List<UserClaim>();

    public virtual ICollection<UserLogin> UserLogins { get; set; } = new List<UserLogin>();

    public virtual ICollection<UserToken> UserTokens { get; set; } = new List<UserToken>();

    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

}
