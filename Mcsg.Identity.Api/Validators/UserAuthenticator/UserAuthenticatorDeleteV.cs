using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Common.SeedWork.Extensions;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserAuthenticatorDeleteV : AbstractValidator<UserAuthenticatorDeleteR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserAuthenticatorDeleteV()
    {
        var t = "OtpCode";
        RuleFor(p => p.OtpCode.Triz()).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
