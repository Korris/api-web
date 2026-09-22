using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using Mcsg.Api.Areas.TapShow.Requests;

public class SegmentUpdateV : AbstractValidator<SegmentUpdateR>
{
    public SegmentUpdateV()
    {
        Include(new SegmentFormBaseV());

        RuleFor(p => p.Id).NotEmpty().WithName("Id");
    }
}
