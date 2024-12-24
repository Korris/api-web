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
        RuleFor(p => p.Email).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .EmailAddress().WithMessage("Invalid email format");
    }

    #endregion
}