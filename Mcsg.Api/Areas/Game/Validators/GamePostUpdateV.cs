using FluentValidation;

namespace Mcsg.Api.Areas.Game.Validators;

using Mcsg.Api.Areas.Game.Requests;
using static Common.SeedWork.Constants.Validator;

public class GamePostUpdateV : AbstractValidator<GamePostUpdateR>
{
    public GamePostUpdateV()
    {
        Include(new GamePostFormBaseV());

        var t = "HashId";
        RuleFor(p => p.HashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
