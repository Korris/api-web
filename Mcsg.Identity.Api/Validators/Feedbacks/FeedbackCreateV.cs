using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class FeedbackCreateV : AbstractValidator<FeedbackCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public FeedbackCreateV()
    {
        var t = "Email";
        RuleFor(p => p.Email).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .EmailAddress().WithMessage("Invalid email format");

        t = "Satisfaction";
        RuleFor(p => p.Satisfaction).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}