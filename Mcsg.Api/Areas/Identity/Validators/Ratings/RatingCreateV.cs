using FluentValidation;

namespace Mcsg.Api.Areas.Identity.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class RatingCreateV : AbstractValidator<RatingCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public RatingCreateV()
    {
        var t = "Email";

        t = "Satisfaction";
        RuleFor(p => p.Satisfaction).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}