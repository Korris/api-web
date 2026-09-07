#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using FluentValidation;

namespace Mcsg.Api.Areas.Identity.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserReferralCreateV : AbstractValidator<UserReferralCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserReferralCreateV()
    {
        var t = "ReferralCode";
        RuleFor(p => p.ReferralCode).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
