using FluentValidation;

namespace Mcsg.Api.Areas.Game.Validators;

using Mcsg.Api.Areas.Game.Requests;

public class GamePostCreateV : AbstractValidator<GamePostCreateR>
{
    public GamePostCreateV()
    {
        Include(new GamePostFormBaseV());
    }
}
