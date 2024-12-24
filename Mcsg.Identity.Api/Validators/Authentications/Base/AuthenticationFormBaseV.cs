using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Common.SeedWork.Constants;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationFormBaseV : AbstractValidator<AuthenticationFormBaseR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationFormBaseV()
    {
        When(p => string.IsNullOrWhiteSpace(p.Email) && string.IsNullOrWhiteSpace(p.Phone), () =>
        {
            var t = nameof(Email);
            RuleFor(p => p.Email).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

            t = "Phone";
            RuleFor(p => p.Phone).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
        });

        When(p => !string.IsNullOrWhiteSpace(p.Phone), () =>
        {
            var t = "Phone";
            RuleFor(p => p.Phone).Matches(Regular.PhoneNumber).WithMessage($"{t} {EmailAddress}");
        });
    }

    #endregion
}
