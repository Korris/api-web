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

namespace Mcsg.Common.SeedWork.Extensions;

using Exceptions;
using Interfaces;

/// <summary>
/// IBusinessRule extension for using [this IBusinessRule] only
/// </summary>
public static class IBusinessRuleExtension
{
    #region -- Methods --

    /// <summary>
    /// Check rule
    /// </summary>
    /// <param name="rule">Broken rule</param>
    /// <exception cref="BrokenRuleException">Broken rule exception</exception>
    public static void CheckRule(this IBusinessRule rule)
    {
        if (rule.IsBroken())
        {
            throw new BrokenRuleException(rule);
        }
    }

    /// <summary>
    /// Is broken
    /// </summary>
    /// <param name="rule">Broken rule</param>
    /// <returns>Return the result</returns>
    public static bool IsBroken(this IBusinessRule rule) => rule.IsBroken();

    #endregion
}
