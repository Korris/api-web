using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Identity.Api.Commands;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Interfaces;
using Requests;
using Validators;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class UserNameUpdateH : BaseSettingH, IRequestHandler<UserNameUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public UserNameUpdateH(IMcsgContext context, ISetting setting) : base(context, setting) { }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    /// <exception cref="BadRequestException"></exception>
    public async Task<SingleResponse> Handle(UserNameUpdateR request, CancellationToken cancellationToken)
    {
        var res = new SingleResponse();

        var vr = new UserNameUpdateV().Validate(request);
        if (!vr.IsValid)
        {
            var t = vr.Errors.ToDic();
            res.SetError(E000, M000, t);
            return res;
        }

        var newUserName = request.NewUserName.Triz();
        if (request.UserName == newUserName)
        {
            return res;
        }

        #region -- Validate on server --
        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.UserId).ToCamelCase(), Value = request.UserId + "" } };
            res.SetError(E002, M002, t);
            return res;
        }

        // UserNameHistory
        var hasUserNameHistory = await _context.UserNameHistoryAvailable.AnyAsync(p => p.UserId != request.UserId && p.UserName == newUserName, cancellationToken);
        if (hasUserNameHistory)
        {
            res.SetError(E107, M107);
            return res;
        }

        var minTimeWaitingChange = _setting.UserNameWaitingChangedAfter;
        var changeRemaining = _setting.UserNameChangedInRemaining;

        var createdOns = await _context.UserNameHistoryAvailable.Where(p => p.UserId == user.Id).Select(p => p.CreatedOn).ToListAsync(cancellationToken);
        var latestChanged = createdOns.OrderByDescending(p => p).FirstOrDefault();
        var previousChanged = createdOns.OrderByDescending(p => p).Skip(1).FirstOrDefault();
        var modifiedCount = createdOns.Count;

        var utcNow = DateTime.UtcNow;
        var remainingTime = utcNow - latestChanged.AddMinutes(changeRemaining);
        var waitTime = utcNow - latestChanged.AddMinutes(minTimeWaitingChange);
        var timePassed = utcNow - latestChanged;
        var timePreviousPassed = utcNow - previousChanged.AddMinutes(changeRemaining);

        var canUpdateUserName = remainingTime.TotalMinutes <= 0;
        var updatedUserName = (timePreviousPassed.TotalMinutes - remainingTime.TotalMinutes) <= changeRemaining;
        var time = waitTime.ToString(@"hh\:mm\:ss");

        #endregion

        if (timePassed.TotalMinutes < minTimeWaitingChange)
        {
            if ((modifiedCount == 2 && !canUpdateUserName) || ((!canUpdateUserName || updatedUserName) && modifiedCount > 2))
            {
                res.SetError(E128, M128 + time);
                return res;
            }
        }

        var userNameHistory = await _context.UserNameHistories.FirstOrDefaultAsync(p => p.UserName == newUserName, cancellationToken);
        if (userNameHistory != null)
        {
            await _context.UserNameHistories
            .Where(p => p.UserName == newUserName)
            .OrderByDescending(p => p.CreatedOn)
            .Take(1)
            .ExecuteUpdateAsync(setters => setters.SetProperty(p => p.IsDelete, true), cancellationToken);
        }

        userNameHistory = new UserNameHistory
        {
            UserId = user.Id,
            UserName = newUserName,
            CreatedBy = user.Id
        };

        await _context.UserNameHistories.AddAsync(userNameHistory);
        user.UserName = newUserName;
        user.NormalizedUserName = newUserName.ToUpper();

        await _context.SaveChangesAsync(cancellationToken);
        res.SetSuccess(newUserName);

        return res;
    }

    #endregion
}
