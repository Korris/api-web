using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class AuthenticationChangeRoleV : AbstractValidator<AuthenticationChangeRoleR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public AuthenticationChangeRoleV()
    {
        var t = "TargetUserId";
        RuleFor(p => p.TargetUserId).NotNull().NotEqual(Guid.Empty).WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "RoleName";
        RuleFor(p => p.RoleName).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
