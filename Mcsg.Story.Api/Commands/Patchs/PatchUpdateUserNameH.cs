using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Story.Api.Commands;

using Common.SeedWork.Responses;
using Interfaces;
using Lib.Data;
using Lib.Data.Domain.Entities;
using Lib.Data.Interfaces;
using Requests;

/// <summary>
/// Handler
/// </summary>
public class PatchUpdateUserNameH : IRequestHandler<PatchUpdateUserNameR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="uniquenessChecker">Uniqueness checker</param>
    public PatchUpdateUserNameH(McsgDbContext context, ISetting setting, IUserNameUniquenessChecker uniquenessChecker)
    {
        _context = context;
        _setting = setting;
        _uniquenessChecker = uniquenessChecker;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(PatchUpdateUserNameR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        // Find Users not listed in UserNameHistories
        var qUser = from a in _context.Users
                    join b in _context.UserNameHistories on a.Id equals b.UserId into g
                    from b in g.DefaultIfEmpty()
                    where b == null
                    select a;

        var users = await qUser.ToListAsync(cancellationToken);
        foreach (var user in users)
        {
            user.UserName = await GenerateUserName(user.Id);
            user.NormalizedUserName = user.UserName!.ToUpper();
            user.ProfileName = user.UserName;
            user.ProfileId = user.UserName;
        }

        await _context.SaveChangesAsync();

        var data = $"Update {users.Count} record(s)";
        res.SetSuccess(data);

        return res;
    }

    /// <summary>
    /// Generate a username without saving changes
    /// </summary>
    /// <param name="userId">UserId</param>
    /// <returns>Return a username</returns>
    private async Task<string?> GenerateUserName(Guid userId)
    {
        var ett = UserNameHistory.Create(_uniquenessChecker, userId);
        await _context.UserNameHistories.AddAsync(ett);
        return ett.UserName;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly McsgDbContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Uniqueness checker
    /// </summary>
    private readonly IUserNameUniquenessChecker _uniquenessChecker;

    #endregion
}
