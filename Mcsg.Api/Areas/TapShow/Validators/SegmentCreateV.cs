using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

public class SegmentCreateV : AbstractValidator<SegmentCreateR>
{
    public SegmentCreateV()
    {
        Include(new SegmentFormBaseV());

        var t = "ChapterHashId";
        RuleFor(p => p.ChapterHashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
