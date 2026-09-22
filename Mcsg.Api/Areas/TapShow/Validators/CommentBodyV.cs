using FluentValidation;

namespace Mcsg.Api.Areas.TapShow.Validators;

using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Comment body rules shared by create / update
/// </summary>
public class CommentBodyV : AbstractValidator<string?>
{
    public CommentBodyV()
    {
        var t = "Body";
        RuleFor(p => p).NotEmpty().WithMessage($"{t} {NotEmpty}").WithName(t)
            .MaximumLength(Comment.Max).WithMessage($"{t} {MaximumLength} {Comment.Max}");
    }
}
