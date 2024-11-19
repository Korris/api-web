using FluentValidation;

namespace Mcsg.Social.Api.Validators;

using Requests;
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
        RuleFor(p => p.Id).NotEmpty().WithMessage($"{t} {NotEmpty}");

        t = "EntityType";
        RuleFor(p => p.NotificationType).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
