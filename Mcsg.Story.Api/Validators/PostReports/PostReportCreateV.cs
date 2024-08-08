using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Common.Core.Enums;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PostReportCreateV : AbstractValidator<PostReportCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PostReportCreateV()
    {
        RuleFor(p => p.ReasonType).NotEmpty().WithMessage($"ReasonType {NotEmpty}");
        RuleFor(p => p.PostId).NotEmpty().WithMessage($"PostId {NotEmpty}");

        When(p => p.ReasonType == ReasonType.Other.ToString(), () =>
        {
            RuleFor(p => p.ReasonText).NotEmpty().WithMessage($"ReasonText {NotEmpty}");
        });
    }

    #endregion
}