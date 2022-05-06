﻿using System.ComponentModel.DataAnnotations;
using LiWiMus.Core.Constants;
using LiWiMus.Core.Tracks;

namespace LiWiMus.Core.Genres;

public class Genre : BaseEntity
{
    [StringLength(50)]
    [RegularExpression(RegularExpressions.DisableTags)]
    public string Name { get; set; } = null!;

    public virtual ICollection<Track> Tracks { get; set; } = null!;
}