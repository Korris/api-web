#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 08:37
 * Update       : 2024-Jan-21 08:37
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

using FluentValidation;

namespace Mcsg.Common.Core.Validators;

using Requests;
using static SeedWork.Constants.Validator;

/// <summary>
/// IdBase validator
/// </summary>
public class IdBaseV : AbstractValidator<IdBaseR>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public IdBaseV()
    {
        var t = "EncryptedId";
        RuleFor(p => p.EncryptedId).NotEmpty().WithMessage($"{t} {NotEmpty}");
    }

    #endregion
}
