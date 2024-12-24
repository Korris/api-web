using FluentValidation;

namespace Mcsg.Document.Api.Validators;

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
        RuleFor(p => p.EntityId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "EntityType";
        RuleFor(p => p.EntityType).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        When(p => p.ReasonType == ReasonType.Other.ToString(), () =>
        {
            t = "Reason";
            RuleFor(p => p.ReasonText).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
        });
    }

    #endregion
}
