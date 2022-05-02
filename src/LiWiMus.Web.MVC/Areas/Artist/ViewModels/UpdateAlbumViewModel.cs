﻿#region

using LiWiMus.Web.Shared;

#endregion

namespace LiWiMus.Web.MVC.Areas.Artist.ViewModels;

public class UpdateAlbumViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public ImageFormFile? Cover { get; set; }
    public DateOnly PublishedAt { get; set; }

    public int[]? ArtistsIds { get; set; }
}