using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class VerificationVerifyCaptchaV : AbstractValidator<VerificationVerifyCaptchaR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public VerificationVerifyCaptchaV()
    {
        var t = "Token";
        RuleFor(p => p.Token).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
