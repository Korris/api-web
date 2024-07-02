using FluentValidation;

namespace Mcsg.Social.Api.Validators;

using Models;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserUserProfileUpdateV : AbstractValidator<UserProfileUpdateRequest>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserUserProfileUpdateV()
    {
        var t = nameof(ProfileName);
        RuleFor(p => p.ProfileName).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .MinimumLength(ProfileName.Min).WithMessage($"{t} {MinimumLength} {ProfileName.Min}")
            .MaximumLength(ProfileName.Max).WithMessage($"{t} {MaximumLength} {ProfileName.Max}");
    }

    #endregion
}
