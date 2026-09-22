using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Shape rules for replacing a segment's choices (existence of targets is checked in the service)
/// </summary>
public class SegmentChoicesSetV : AbstractValidator<SegmentChoicesSetR>
{
    public SegmentChoicesSetV()
    {
        RuleFor(p => p.Id).NotEmpty().WithName("Id");

        var t = "Choices";
        RuleFor(p => p.Choices).Cascade(CascadeMode.Stop).NotNull().WithName(t)
            .Must(c => c.Count <= TapShowConfig.MaxChoicesPerSegment)
            .WithMessage($"{t} max {TapShowConfig.MaxChoicesPerSegment} items")
            .Must(c => c.Select(x => x.TargetSegmentId).Distinct().Count() == c.Count)
            .WithMessage(TapShowConfig.InvalidChoiceTargetMessage);

        RuleForEach(p => p.Choices).ChildRules(c =>
        {
            var l = "Label";
            c.RuleFor(x => x.Label).NotEmpty().WithMessage($"{l} {NotEmpty}").WithName(l)
                .MaximumLength(Title.Max).WithMessage($"{l} {MaximumLength} {Title.Max}");
            c.RuleFor(x => x.TargetSegmentId).NotEmpty().WithName("TargetSegmentId");
        });
    }
}
