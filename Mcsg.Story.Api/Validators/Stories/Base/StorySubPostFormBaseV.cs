using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class StorySubPostFormBaseV : AbstractValidator<StorySubPostFormBaseR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public StorySubPostFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}")
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");
    }

    #endregion
}
