using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Common.SeedWork.Extensions;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserNameUpdateV : AbstractValidator<UserNameUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserNameUpdateV()
    {
        var t = "";
        When(p => p.IsPremium, () =>
        {
            t = nameof(UserNamePremium);
            RuleFor(p => p.NewUserName.Triz())
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MinimumLength(UserNamePremium.Min).WithMessage($"{t} {MinimumLength} {UserNamePremium.Min}")
            .MaximumLength(UserNamePremium.Max).WithMessage($"{t} {MaximumLength} {UserNamePremium.Max}")
            .Matches(UserNamePremium.Regex).WithMessage($"{t} {UserNamePremium.Message}");
        });

        When(p => !p.IsPremium, () =>
        {
            t = nameof(UserNameFree);
            RuleFor(p => p.NewUserName.Triz())
            .Cascade(CascadeMode.Stop)
            .NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MinimumLength(UserNameFree.Min).WithMessage($"{t} {MinimumLength} {UserNameFree.Min}")
            .MaximumLength(UserNameFree.Max).WithMessage($"{t} {MaximumLength} {UserNameFree.Max}")
            .Matches(UserNameFree.Regex).WithMessage($"{t} {UserNameFree.Message}");
        });

    }

    #endregion
}
