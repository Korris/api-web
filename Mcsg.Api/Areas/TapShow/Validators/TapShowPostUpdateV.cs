using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;
using static Common.SeedWork.Constants.Validator;

public class TapShowPostUpdateV : AbstractValidator<TapShowPostUpdateR>
{
    public TapShowPostUpdateV()
    {
        Include(new TapShowPostFormBaseV());

        var t = "HashId";
        RuleFor(p => p.HashId).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t);
    }
}
