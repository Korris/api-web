using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class DocumentSubPostFormBaseV : AbstractValidator<DocumentSubPostFormBaseR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public DocumentSubPostFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");
    }

    #endregion
}
