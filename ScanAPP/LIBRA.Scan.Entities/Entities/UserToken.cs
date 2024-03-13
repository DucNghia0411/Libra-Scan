﻿using System;
using System.Collections.Generic;

namespace LIBRA.Scan.Entities.Entities;

public partial class UserToken
{
    public long UserId { get; set; }

    public string LoginProvider { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Value { get; set; }

    public virtual User User { get; set; } = null!;
}
