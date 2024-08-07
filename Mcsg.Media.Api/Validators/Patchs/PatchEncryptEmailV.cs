using FluentValidation;

namespace Mcsg.Media.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PatchEncryptEmailV : AbstractValidator<PatchEncryptEmailR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchEncryptEmailV()
    {
        var t = "Otp";
        RuleFor(p => p.Otp).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
