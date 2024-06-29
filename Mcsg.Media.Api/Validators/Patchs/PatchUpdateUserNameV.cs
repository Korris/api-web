using FluentValidation;

namespace Mcsg.Media.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PatchUpdateUserNameV : AbstractValidator<PatchUpdateUserNameR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchUpdateUserNameV()
    {
        var t = "Otp";
        RuleFor(p => p.Otp).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
