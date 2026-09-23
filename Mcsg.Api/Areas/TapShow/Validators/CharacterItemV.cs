using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validation of one inline character in the "characters" array of POST / PUT api/tapshow/tapshow
/// </summary>
public class CharacterItemV : AbstractValidator<CharacterItemR>
{
    public CharacterItemV()
    {
        var t = "Characters.Name";
        RuleFor(p => p.Name).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Title.Max).WithMessage($"{t} {MaximumLength} {Title.Max}");

        t = "Characters.AvatarHashId";
        RuleFor(p => p.AvatarHashId).MaximumLength(Hashtag.Max).WithMessage($"{t} {MaximumLength} {Hashtag.Max}").WithName(t);
    }
}
