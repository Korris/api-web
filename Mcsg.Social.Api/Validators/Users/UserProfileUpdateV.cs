using FluentValidation;

namespace Mcsg.Social.Api.Validators;

using Common.SeedWork.Constants;
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
            .Matches(Regular.ProfileName).WithMessage($"{t} {Regular.RegexMessage}");
    }

    #endregion
}
