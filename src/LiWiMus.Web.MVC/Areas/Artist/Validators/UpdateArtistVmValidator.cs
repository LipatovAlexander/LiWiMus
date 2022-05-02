﻿#region

using ByteSizeLib;
using FluentValidation;
using LiWiMus.Web.MVC.Areas.Artist.ViewModels;
using LiWiMus.Web.Shared.Extensions;

#endregion

namespace LiWiMus.Web.MVC.Areas.Artist.Validators;

public class UpdateArtistVmValidator : AbstractValidator<UpdateArtistViewModel>
{
    public UpdateArtistVmValidator()
    {
        RuleFor(model => model.Name)
            .NotNull()
            .NotEmpty()
            .MaximumLength(50)
            .DisableTags();

        RuleFor(model => model.About)
            .NotNull()
            .NotEmpty()
            .MaximumLength(500)
            .DisableTags();

        RuleFor(model => model.Photo!)
            .SidesPercentageDifferenceMustBeLessThan(10)
            .MustWeightLessThan(ByteSize.FromMegaBytes(1))
            .When(model => model.Photo is not null);
    }
}