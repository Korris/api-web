using FluentValidation;

namespace Mcsg.Api.Areas.Social.Validators;

using Mcsg.Api.Areas.Social.Requests;
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
