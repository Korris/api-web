using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

public class CharacterCreateV : AbstractValidator<CharacterCreateR>
{
    public CharacterCreateV()
    {
        Include(new CharacterFormBaseV());

        var t = "PostHashId";
        RuleFor(p => p.PostHashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
