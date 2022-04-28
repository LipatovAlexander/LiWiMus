﻿using LiWiMus.Core.Models;

namespace LiWiMus.Web.Areas.Artist.ViewModels;

public class UpdateAlbumViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public ImageInfo? Cover { get; set; }
    public DateOnly PublishedAt { get; set; }
    
    public int[]? ArtistsIds { get; set; }
}