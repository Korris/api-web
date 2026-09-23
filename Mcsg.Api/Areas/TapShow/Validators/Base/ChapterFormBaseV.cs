using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Common.Core.Enums;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Shared validation rules for create / update chapter
/// </summary>
public class ChapterFormBaseV : AbstractValidator<ChapterFormBaseR>
{
    public ChapterFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

        // Only Draft / Public are meaningful for an author-managed chapter
        t = "Status";
        RuleFor(p => p.Status).Must(s => s == PostStatus.Draft || s == PostStatus.Public)
            .WithMessage($"{t} must be Draft or Public").WithName(t);

        t = "ThumbnailHashId";
        RuleFor(p => p.ThumbnailHashId).MaximumLength(Hashtag.Max).WithMessage($"{t} {MaximumLength} {Hashtag.Max}").WithName(t);
    }
}
