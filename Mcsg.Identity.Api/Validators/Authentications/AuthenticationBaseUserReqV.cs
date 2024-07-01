using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Common.SeedWork.Constants;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationBaseUserReqV : AbstractValidator<BaseUserReq>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationBaseUserReqV()
    {
        When(p => string.IsNullOrWhiteSpace(p.Email) && string.IsNullOrWhiteSpace(p.Phone), () =>
        {
            var t = nameof(Email);
            RuleFor(p => p.Email).NotEmpty().WithMessage($"{t} {NotEmpty}");

            t = "Phone";
            RuleFor(p => p.Phone).NotEmpty().WithMessage($"{t} {NotEmpty}");
        });

        When(p => !string.IsNullOrWhiteSpace(p.Email), () =>
        {
            var t = nameof(Email);
            RuleFor(p => p.Email).NotEmpty().WithMessage($"{t} {NotEmpty}")
                .MinimumLength(Email.Min).WithMessage($"{t} {MinimumLength} {Email.Min}")
                .MaximumLength(Email.Max).WithMessage($"{t} {MaximumLength} {Email.Max}")
                .EmailAddress().WithMessage($"{t} {EmailAddress}");
        });

        When(p => !string.IsNullOrWhiteSpace(p.Phone), () =>
        {
            var t = "Phone";
            RuleFor(p => p.Phone).Matches(Regular.PhoneNumber).WithMessage($"{t} {EmailAddress}");
        });
    }

    #endregion
}
