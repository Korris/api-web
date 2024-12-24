using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationDeleteUserV : AbstractValidator<AuthenticationDeleteUserR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationDeleteUserV()
    {
        var t = nameof(Password);
        RuleFor(p => p.Password).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
