using FluentValidation;

namespace Mcsg.Api.Areas.Identity.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

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

        When(p => p.IsForAdmin, () =>
        {
            var t = nameof(Password);
            RuleFor(p => p.Password).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
                .MinimumLength(Password.Min).WithMessage($"{t} {MinimumLength} {Password.Min}")
                .MaximumLength(Password.Max).WithMessage($"{t} {MaximumLength} {Password.Max}");

            t = "ConfirmPassword";
            RuleFor(p => p.ConfirmPassword).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
                .MinimumLength(Password.Min).WithMessage($"{t} {MinimumLength} {Password.Min}")
                .MaximumLength(Password.Max).WithMessage($"{t} {MaximumLength} {Password.Max}")
                .Equal(x => x.Password).WithMessage($"{t} {Equal}");
        });
    }

    #endregion
}
