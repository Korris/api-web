using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;

public class TapShowPostCreateV : AbstractValidator<TapShowPostCreateR>
{
    public TapShowPostCreateV()
    {
        Include(new TapShowPostFormBaseV());
    }
}
