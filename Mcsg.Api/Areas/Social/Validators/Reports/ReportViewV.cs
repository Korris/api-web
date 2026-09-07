using FluentValidation;

namespace Mcsg.Api.Areas.Social.Validators;

using Mcsg.Api.Areas.Social.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class ReportViewV : AbstractValidator<ReportViewR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ReportViewV()
    {
        var t = "EntityId";
        RuleFor(p => p.Id).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "EntityType";
        RuleFor(p => p.NotificationType).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
