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
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

        t = "Order";
        RuleFor(p => p.Order).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "PublishDate";
        RuleFor(p => p.PublishDate).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "Permission";
        RuleFor(p => p.Permission).NotNull().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
