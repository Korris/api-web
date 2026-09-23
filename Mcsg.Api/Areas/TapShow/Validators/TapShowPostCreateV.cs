using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Constants;
using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

public class TapShowPostCreateV : AbstractValidator<TapShowPostCreateR>
{
    public TapShowPostCreateV()
    {
        Include(new TapShowPostFormBaseV());

        // Inline characters (optional): bounded count, each item validated on its own
        var t = "Characters";
        RuleFor(p => p.Characters).Must(c => c == null || c.Count <= TapShowConfig.MaxCharactersPerPost)
            .WithMessage($"{t} {MaximumLength} {TapShowConfig.MaxCharactersPerPost}").WithName(t);
        RuleForEach(p => p.Characters).SetValidator(new CharacterItemV());
    }
}
