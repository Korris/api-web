using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;

public class CharacterUpdateV : AbstractValidator<CharacterUpdateR>
{
    public CharacterUpdateV()
    {
        Include(new CharacterFormBaseV());

        RuleFor(p => p.Id).NotEmpty().WithName("Id");
    }
}
