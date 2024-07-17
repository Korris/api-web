namespace Mcsg.Comic.Api.Checkers;

using Lib.Data;
using Lib.Data.Interfaces;

/// <summary>
/// UserName uniqueness checker
/// </summary>
public class UserNameUniquenessChecker : IUserNameUniquenessChecker
{
    #region -- Implements --

    /// <summary>
    /// Is unique
    /// </summary>
    /// <param name="data">Input data</param>
    /// <returns>Return the result</returns>
    public bool IsUnique(string data)
    {
        return !_context.UserNameHistories.Any(p => p.UserName == data);
    }

    /// <summary>
    /// Next developer name
    /// </summary>
    public string NextDevName => _context.MakeNo(_context.UserNameHistories, p => p.Id, p => p.UserName + "", "");

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserNameUniquenessChecker(McsgDbContext context)
    {
        _context = context;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly McsgDbContext _context;

    #endregion
}
