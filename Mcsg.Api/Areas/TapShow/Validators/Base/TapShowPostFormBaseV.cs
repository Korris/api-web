using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using Mcsg.Api.Validators;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Shared validation rules for create / update TapShow post (copied from GamePostFormBaseV)
/// </summary>
public class TapShowPostFormBaseV : AbstractValidator<TapShowPostFormBaseR>
{
    public TapShowPostFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

        t = "Summary";
        RuleFor(p => p.Summary).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Summary.Max).WithMessage($"{t} {MaximumLength} {Summary.Max}");

        t = "Permission";
        RuleFor(p => p.Permission).IsInEnum().WithMessage($"{t} {NotEmpty}").WithName(t);

        t = "ThumbnailHashId";
        RuleFor(p => p.ThumbnailHashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Hashtag.Max).WithMessage($"{t} {MaximumLength} {Hashtag.Max}");

        // AuthorName only matters when the poster is not the author
        t = "AuthorName";
        RuleFor(p => p.AuthorName).NotEmpty().When(p => !p.IsCurrentUserAuthor).WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

        // Hashtags: same rules as Comic (shape, no duplicates, max quantity)
        t = "Tags";
        RuleForEach(p => p.Tags).Must(HashtagRules.Valid).WithMessage(Tag);
        RuleFor(p => p.Tags).Must(HashtagRules.NoDuplicate).WithMessage(DuplicateTag).WithName(t)
            .Must(HashtagRules.MaxQuantity).WithMessage($"{t} {LessThanOrEqualTo} {Hashtag.MaxQuantity}");
    }
}
