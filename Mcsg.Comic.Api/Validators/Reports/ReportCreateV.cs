using FluentValidation;

namespace Mcsg.Comic.Api.Validators;

using Common.Core.Enums;
using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class ReportCreateV : AbstractValidator<ReportCreateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ReportCreateV()
    {
        var t = "EntityId";
        RuleFor(p => p.EntityId).NotEmpty().WithMessage($"{t} {NotEmpty}");

        t = "EntityType";
        RuleFor(p => p.EntityType).NotEmpty().WithMessage($"{t} {NotEmpty}");

        When(p => p.ReasonType == ReasonType.Other.ToString(), () =>
        {
            t = "Reason";
            RuleFor(p => p.ReasonText).NotEmpty().WithMessage($"{t} {NotEmpty}");
        });
    }

    #endregion
}
