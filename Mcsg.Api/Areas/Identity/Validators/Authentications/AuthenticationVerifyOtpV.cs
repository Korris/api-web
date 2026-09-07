using FluentValidation;

namespace Mcsg.Api.Areas.Identity.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationVerifyOtpV : AbstractValidator<AuthenticationVerifyOtpR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationVerifyOtpV()
    {
        var t = nameof(OtpCode);
        RuleFor(p => p.OtpCode).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
