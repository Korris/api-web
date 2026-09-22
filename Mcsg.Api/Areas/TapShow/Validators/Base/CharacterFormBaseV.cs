using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Shared validation rules for create / update character
/// </summary>
public class CharacterFormBaseV : AbstractValidator<CharacterFormBaseR>
{
    public CharacterFormBaseV()
    {
        var t = "Name";
        RuleFor(p => p.Name).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

        t = "AvatarHashId";
        RuleFor(p => p.AvatarHashId).MaximumLength(Hashtag.Max).WithMessage($"{t} {MaximumLength} {Hashtag.Max}").WithName(t);
    }
}
