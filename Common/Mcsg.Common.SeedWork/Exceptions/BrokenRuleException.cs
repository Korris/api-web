#region Information
/*
 * Author       : Toan Nguyen Van
 * Email        : nvt87x@gmail.com
 * Phone        : +84 345 515 010
 * ------------------------------- *
 * Create       : 2024-Jan-21 07:03
 * Update       : 2024-Jan-21 07:03
 * Checklist    : 1.0
 * Status       : New
 */
#endregion

namespace Mcsg.Common.SeedWork.Exceptions;

using Interfaces;

/// <summary>
/// Broken rule exception
/// </summary>
/// <remarks>
/// Initialize
/// </remarks>
/// <param name="rule">Rule</param>
public class BrokenRuleException(IBusinessRule rule) : Exception(rule.Message)
{
    #region -- Overrides --

    /// <summary>
    /// To string
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        return $"{Rule.GetType().FullName}:\n{Rule.Message}";
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// Broken rule
    /// </summary>
    public IBusinessRule Rule { get; } = rule;

    /// <summary>
    /// The message that describes the error
    /// </summary>
    public string Details { get; } = rule.Message;

    #endregion
}
