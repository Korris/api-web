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

namespace Mcsg.Common.Core.Rules;

using Interfaces;
using SeedWork.Interfaces;
using static SeedWork.Constants.Error;

/// <summary>
/// Must be unique rule
/// </summary>
public class MustBeUniqueRule : IBusinessRule
{
    #region -- Implements --

    /// <summary>
    /// Is broken
    /// </summary>
    /// <returns>Return the result</returns>
    public bool IsBroken() => !_uniquenessChecker.IsUnique(_data);

    /// <summary>
    /// Next developer name
    /// </summary>
    public string NextDevName => _uniquenessChecker.NextDevName;

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="uniquenessChecker">Uniqueness checker</param>
    /// <param name="data">Input data</param>
    public MustBeUniqueRule(IUniquenessChecker uniquenessChecker, string data)
    {
        _uniquenessChecker = uniquenessChecker;
        _data = data;
    }

    #endregion

    #region -- Properties --

    /// <summary>
    /// The message that describes the error
    /// </summary>
    public string Message => E107;

    #endregion

    #region -- Fields --

    /// <summary>
    /// Uniqueness checker
    /// </summary>
    private readonly IUniquenessChecker _uniquenessChecker;

    /// <summary>
    /// Input data
    /// </summary>
    private readonly string _data;

    #endregion
}
