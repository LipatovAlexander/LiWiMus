﻿using LiWiMus.Core.Albums;
using LiWiMus.Core.Artists;
using LiWiMus.Core.Playlists;
using LiWiMus.Core.PlaylistTracks;
using LiWiMus.Core.Tracks;
using LiWiMus.Core.Users;
using LiWiMus.Core.Users.Enums;
using LiWiMus.SharedKernel.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace LiWiMus.Infrastructure.Data.Seeders;

// ReSharper disable once UnusedType.Global
public class PlaylistSeeder : ISeeder
{
    private readonly IRepository<Artist> _artistRepository;
    private readonly IRepository<Track> _trackRepository;
    private readonly IRepository<Album> _albumRepository;
    private readonly IRepository<Playlist> _playlistRepository;
    private readonly UserManager<User> _userManager;
    private readonly IRepository<PlaylistTrack> _playlistTrackRepository;

    public PlaylistSeeder(IRepository<Artist> artistRepository, IRepository<Track> trackRepository, 
        IRepository<Album> albumRepository, IRepository<Playlist> playlistRepository, UserManager<User> userManager, IRepository<PlaylistTrack> playlistTrackRepository)
    {
        _artistRepository = artistRepository;
        _trackRepository = trackRepository;
        _albumRepository = albumRepository;
        _playlistRepository = playlistRepository;
        _userManager = userManager;
        _playlistTrackRepository = playlistTrackRepository;
    }
    public async Task SeedAsync(EnvironmentType environmentType)
    {
        const string userName = "MockUser_Playlist";
        
        if (await _userManager.FindByNameAsync(userName) is not null)
        {
            return;
        }
        
        switch (environmentType)
        {
            case EnvironmentType.Development:
            case EnvironmentType.Testing:
                var artist = new Artist
                {
                    About = "About",
                    Name = "MockArtist_Playlist",
                    PhotoLocation = "Location"
                };

                await _artistRepository.AddAsync(artist);

                var album = new Album
                {
                    Title = "MockAlbum_Playlist",
                    CoverLocation = "Location",
                    Owners = new List<Artist> {artist}
                };

                await _albumRepository.AddAsync(album);

                var track1 = new Track
                {
                    Album = album,
                    Name = "MockTrack_Playlist",
                    Duration = 190,
                    FileLocation = "Location"
                };
                
                var track2 = new Track
                {
                    Id = 100,
                    Album = album,
                    Name = "MockTrack_Playlist",
                    Duration = 190,
                    FileLocation = "Location"
                };
                
                var track3 = new Track
                {
                    Id = 101,
                    Album = album,
                    Name = "MockTrack_Playlist",
                    Duration = 190,
                    FileLocation = "Location"
                };

                await _trackRepository.AddAsync(track1);
                await _trackRepository.AddAsync(track2);
                await _trackRepository.AddAsync(track3);

                var user = new User
                {
                    UserName = userName,
                    Email = "mockEmail@mock.mock_Playlist",
                    Gender = Gender.Female,
                    AvatarLocation = "Location"
                };

                await _userManager.CreateAsync(user, "Password");

                var playlist = new Playlist
                {
                    Name = "MockPlaylist_Playlist",
                    Owner = user,
                    PhotoLocation = "Location"
                };

                await _playlistRepository.AddAsync(playlist);

                var playlistTrack = new PlaylistTrack
                {
                    Playlist = playlist,
                    Track = track1
                };

                await _playlistTrackRepository.AddAsync(playlistTrack);
                break;
            case EnvironmentType.Production:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(environmentType), environmentType, null);
        }
    }

    public int Priority => 20;
}