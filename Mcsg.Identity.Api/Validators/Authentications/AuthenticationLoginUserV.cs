using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

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
        RuleFor(p => p.Password).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
