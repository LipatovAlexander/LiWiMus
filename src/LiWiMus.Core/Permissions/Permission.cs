﻿using System.Security.Claims;
using LiWiMus.Core.Plans;

namespace LiWiMus.Core.Permissions;

public class Permission
{
    public const string ClaimType = "permission";

    public int Id { get; set; }
    public string Name { get; set; }

    public virtual ICollection<Plan> Plans { get; set; } = null!;

    public Permission(string name)
    {
        Name = name;
    }

    public Claim ToClaim()
    {
        return new Claim(ClaimType, Name);
    }
}