using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Shared validation rules for create / update segment: image and narration are both optional but not both empty
/// </summary>
public class SegmentFormBaseV : AbstractValidator<SegmentFormBaseR>
{
    public SegmentFormBaseV()
    {
        var t = "Title";
        RuleFor(p => p.Title).MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}").WithName(t);

        t = "ImageHashId";
        RuleFor(p => p.ImageHashId).MaximumLength(Hashtag.Max).WithMessage($"{t} {MaximumLength} {Hashtag.Max}").WithName(t);

        t = "AudioHashId";
        RuleFor(p => p.AudioHashId).MaximumLength(Hashtag.Max).WithMessage($"{t} {MaximumLength} {Hashtag.Max}").WithName(t);

        t = "Narration";
        RuleFor(p => p.Narration).MaximumLength(TapShowConfig.NarrationMaxLength)
            .WithMessage($"{t} {MaximumLength} {TapShowConfig.NarrationMaxLength}").WithName(t);

        RuleFor(p => p).Must(p => !string.IsNullOrWhiteSpace(p.ImageHashId) || !string.IsNullOrWhiteSpace(p.Narration))
            .WithMessage(TapShowConfig.EmptySegmentMessage).WithName("Segment");
    }
}
