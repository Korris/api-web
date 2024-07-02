using FluentValidation;

namespace Mcsg.Media.Api.Validators;

using Requests;
using static Common.SeedWork.Constants.Validator;

/// <summary>
/// Validator
/// </summary>
public class PatchMoveFolderV : AbstractValidator<PatchMoveFolderR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public PatchMoveFolderV()
    {
        var t = "Otp";
        RuleFor(p => p.Otp).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
