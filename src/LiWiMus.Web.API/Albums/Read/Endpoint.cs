﻿using AutoMapper;
using LiWiMus.Core.Albums;
using LiWiMus.Core.Albums.Specifications;
using LiWiMus.Core.Artists;
using LiWiMus.SharedKernel.Interfaces;
using LiWiMus.Web.API.Shared;
using LiWiMus.Web.API.Shared.Extensions;
using LiWiMus.Web.Shared.Extensions;
using MinimalApi.Endpoint;

namespace LiWiMus.Web.API.Albums.Read;

// ReSharper disable once UnusedType.Global
public class Endpoint : IEndpoint<IResult, int>
{
    private IRepository<Album> _albumRepository = null!;
    private readonly IMapper _mapper;

    public Endpoint(IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task<IResult> HandleAsync(int id)
    {
        var album = await _albumRepository.GetByIdAsync(id);

        if (album is null)
        {
            return Results.Extensions.NotFoundById(EntityType.Albums, id);
        }

        var dto = _mapper.Map<Dto>(album);
        dto.Artists = _mapper.MapList<Artist, Artists.Dto>(await _albumRepository.GetArtistsAsync(album)).ToList();
        dto.TracksCount = await _albumRepository.GetTracksCountAsync(album);
        dto.ListenersCount = await _albumRepository.GetListenersCountAsync(album);

        return Results.Ok(dto);
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapGet(RouteConstants.Albums.Read, async (int id, IRepository<Album> repository) =>
        {
            _albumRepository = repository;
            return await HandleAsync(id);
        });
    }
}