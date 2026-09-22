using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

public class ChapterCreateV : AbstractValidator<ChapterCreateR>
{
    public ChapterCreateV()
    {
        Include(new ChapterFormBaseV());

        var t = "PostHashId";
        RuleFor(p => p.PostHashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
