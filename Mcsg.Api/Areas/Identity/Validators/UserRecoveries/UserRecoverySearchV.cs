using FluentValidation;

namespace Mcsg.Api.Areas.Identity.Validators;
using Mcsg.Api.Areas.Identity.Requests;

using Common.SeedWork.Extensions;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserRecoverySearchV : AbstractValidator<UserRecoverySearchR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserRecoverySearchV()
    {
        var t = "Password";
        RuleFor(p => p.Password.Triz()).Cascade(CascadeMode.Stop).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MinimumLength(Password.Min).WithMessage($"{t} {MinimumLength} {Password.Min}")
            .MaximumLength(Password.Max).WithMessage($"{t} {MaximumLength} {Password.Max}");
    }

    #endregion
}
