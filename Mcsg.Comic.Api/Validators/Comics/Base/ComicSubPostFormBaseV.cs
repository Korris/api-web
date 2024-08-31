using FluentValidation;

namespace Mcsg.Comic.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class ComicSubPostFormBaseV : AbstractValidator<ComicSubPostFormBaseR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public ComicSubPostFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");
    }

    #endregion
}
