using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Requests;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationRegisterUserV : AbstractValidator<AuthenticationRegisterUserR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationRegisterUserV()
    {
        Include(new AuthenticationFormBaseV());
    }

    #endregion
}
