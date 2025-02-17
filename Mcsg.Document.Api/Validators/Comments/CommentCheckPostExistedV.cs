using FluentValidation;

namespace Mcsg.Document.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

public class CommentCheckPostExistedV : AbstractValidator<CommentCheckPostExistedR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public CommentCheckPostExistedV()
    {
        var t = "PostId";
        RuleFor(p => p.PostId).NotNull().WithMessage($"{t} {NotEmpty}").WithName(t);
    }

    #endregion
}
