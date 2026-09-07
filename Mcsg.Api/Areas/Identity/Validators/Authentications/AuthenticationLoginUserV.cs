using FluentValidation;

namespace Mcsg.Api.Areas.Identity.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationLoginUserV : AbstractValidator<AuthenticationLoginUserR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationLoginUserV()
    {
        Include(new AuthenticationFormBaseV());

        var t = nameof(Password);
        RuleFor(p => p.Password).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
