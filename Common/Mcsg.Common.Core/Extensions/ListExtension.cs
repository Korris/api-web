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

using FluentValidation.Results;

namespace Mcsg.Common.Core.Extensions;

using SeedWork.Dtos;
using SeedWork.Extensions;

/// <summary>
/// List extension for using [this List] only
/// </summary>
public static class ListExtension
{
    #region -- Methods --

    /// <summary>
    /// Convert to DicDto
    /// </summary>
    /// <param name="o">Encapsulates an error from the identity subsystem</param>
    /// <param name="isCamelCase">Is camelCase</param>
    /// <returns>Return the list of DicDto</returns>
    public static List<DicDto> ToDic(this List<ValidationFailure> o, bool isCamelCase = true)
    {
        return o.Select(p => new DicDto { Key = isCamelCase ? p.PropertyName.ToCamelCase() : p.PropertyName, Value = p.ErrorMessage }).ToList();
    }

    /// <summary>
    /// Convert to value
    /// </summary>
    /// <param name="o">Encapsulates an error from the identity subsystem</param>
    /// <param name="isCamelCase">Is camelCase</param>
    /// <returns>Return the result</returns>
    public static string ToValue(this List<ValidationFailure> o, bool isCamelCase = true)
    {
        var t = o.ToDic(isCamelCase).Select(p => p.Value).ToList();
        return string.Join(" | ", t);
    }

    #endregion
}
