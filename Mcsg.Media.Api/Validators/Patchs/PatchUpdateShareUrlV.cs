using FluentValidation;

namespace Mcsg.Media.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PatchUpdateShareUrlV : AbstractValidator<PatchUpdateShareUrlR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchUpdateShareUrlV()
    {
        var t = "Otp";
        RuleFor(p => p.Otp).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
