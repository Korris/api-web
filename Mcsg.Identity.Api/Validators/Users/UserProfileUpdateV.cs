using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Common.SeedWork.Extensions;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserProfileUpdateV : AbstractValidator<UserProfileUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserProfileUpdateV()
    {
        var t = nameof(ProfileName);
        RuleFor(p => p.ProfileName.Triz())
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage($"{t} {NotEmpty}")
            .MinimumLength(ProfileName.Min).WithMessage($"{t} {MinimumLength} {ProfileName.Min}")
            .MaximumLength(ProfileName.Max).WithMessage($"{t} {MaximumLength} {ProfileName.Max}")
            .Matches(ProfileName.Regex).WithMessage($"{t} {ProfileName.Message}");

        t = nameof(Location);
        RuleFor(p => p.Location.Triz())
            .Cascade(CascadeMode.Stop)
            .MaximumLength(Location.Max).WithMessage($"{t} {MaximumLength} {Location.Max}")
            .Matches(Location.Regex).WithMessage($"{t} {Location.Message}");
    }

    #endregion
}
