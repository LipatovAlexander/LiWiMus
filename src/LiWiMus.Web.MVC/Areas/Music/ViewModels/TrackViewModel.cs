﻿using LiWiMus.Web.MVC.ViewModels.GeneralInfoViewModels;

namespace LiWiMus.Web.MVC.Areas.Music.ViewModels;

public class TrackViewModel
{
    public ICollection<ArtistGeneralInfoViewModel> Owners { get; set; } = null!;
    public ICollection<GenreGeneralInfoViewModel> Genres { get; set; } = null!;

    public AlbumGeneralInfoViewModel Album { get; set; } = null!;
    
    public string Name { get; set; } = null!;
    
    public string FileLocation { get; set; } = null!;
}