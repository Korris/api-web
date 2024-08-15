using FluentValidation;

namespace Mcsg.Identity.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class UserNameUpdateV : AbstractValidator<UserNameUpdateR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public UserNameUpdateV()
    {
        var t = "NewUserName";
        RuleFor(p => p.NewUserName).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
