﻿using FluentValidation;
using LiWiMus.Web.MVC.Areas.Artist.ViewModels;

namespace LiWiMus.Web.MVC.Areas.Artist.Validators;

public class UpdateAlbumVmValidator : AbstractValidator<UpdateAlbumViewModel>
{
    public UpdateAlbumVmValidator()
    {
        RuleFor(model => model.Id)
            .NotEmpty();
        RuleFor(model => model.Title)
            .NotEmpty()
            .MaximumLength(50);
        RuleFor(model => model.PublishedAt)
            .NotEmpty();

        RuleFor(model => model.Cover)
            .Must(cover => cover!.ContentType.StartsWith("image"))
            .When(model => model.Cover is not null)
            .WithMessage("Bad image");
    }
}