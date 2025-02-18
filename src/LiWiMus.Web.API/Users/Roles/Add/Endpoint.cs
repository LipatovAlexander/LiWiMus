﻿using LiWiMus.Core.Roles;
using LiWiMus.Core.Roles.Interfaces;
using LiWiMus.Core.Users;
using LiWiMus.SharedKernel.Interfaces;
using LiWiMus.Web.API.Shared;
using LiWiMus.Web.API.Shared.Extensions;
using MinimalApi.Endpoint;

namespace LiWiMus.Web.API.Users.Roles.Add;

public class Endpoint : IEndpoint<IResult, Request>
{
    private IRepository<User> _userRepository = null!;
    private IRepository<Role> _roleRepository = null!;
    private IRoleManager _roleManager = null!;

    public async Task<IResult> HandleAsync(Request request)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user is null)
        {
            return Results.Extensions.NotFoundById(EntityType.Users, request.UserId);
        }

        var role = await _roleRepository.GetByIdAsync(request.RoleId);
        if (role is null)
        {
            return Results.Extensions.NotFoundById(EntityType.Roles, request.RoleId);
        }

        if (await _roleManager.IsInRoleAsync(user, role))
        {
            return Results.Conflict("User already in this role");
        }

        await _roleManager.AddToRoleAsync(user, role);
        return Results.Ok();
    }

    public void AddRoute(IEndpointRouteBuilder app)
    {
        app.MapPost(RouteConstants.Users.Roles.Add,
            async (Request request, IRepository<User> userRepository, IRepository<Role> roleRepository,
                   IRoleManager roleManager) =>
            {
                _userRepository = userRepository;
                _roleRepository = roleRepository;
                _roleManager = roleManager;
                return await HandleAsync(request);
            });
    }
}