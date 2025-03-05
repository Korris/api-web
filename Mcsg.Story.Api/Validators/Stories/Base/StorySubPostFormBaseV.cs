using FluentValidation;

namespace Mcsg.Story.Api.Validators;

using Common.Core.Enums;
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
        RuleFor(p => p.Order).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .InclusiveBetween(ChapterRange.Min, ChapterRange.Max).WithMessage($"{t} {GreaterThan} {ChapterRange.Min} and {LessThan} {ChapterRange.Max}")
            .Must(order => ValidateOrder(order ?? 0))
            .WithMessage($"{t} {LessThan} 2 decimal places.");

        t = "PublishDate";
        RuleFor(p => p.PublishDate).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "Permission";
        RuleFor(p => p.Permission).NotNull().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "Status";
        RuleFor(p => p.Status).NotNull().WithMessage($"{t} {NotEmpty}")
            .Must(status => status == PostStatus.Draft || status == PostStatus.Public)
            .WithMessage(InvalidStatus).WithName(t);
    }

    /// <summary>
    /// ValidateOrder
    /// </summary>
    /// <param name="order">The order of the subpost</param>
    /// <returns>Return the result</returns>
    public bool ValidateOrder(float order)
    {
        string orderStr = order.ToString();
        int decimalIndex = orderStr.IndexOf('.');
        if (decimalIndex == -1)
        {
            return true;
        }

        string decimalPart = orderStr.Substring(decimalIndex + 1);

        return decimalPart.Length <= 2;
    }

    #endregion
}
