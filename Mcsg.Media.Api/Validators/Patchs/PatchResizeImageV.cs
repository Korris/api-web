using FluentValidation;

namespace Mcsg.Media.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PatchResizeImageV : AbstractValidator<PatchResizeImageR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchResizeImageV()
    {
        var t = "Otp";
        RuleFor(p => p.Otp).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
