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
using Requests;
using Validators;
using static Common.SeedWork.Constants.Message;

/// <summary>
/// Handler
/// </summary>
public class UserNameUpdateH : BaseH, IRequestHandler<UserNameUpdateR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserNameUpdateH(IMcsgContext context) : base(context) { }

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
            res.SetError(t, M000);
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
            res.SetError(t, M002);
            return res;
        }

        var createdOns = await _context.UserNameHistoryAvailable.Where(p => p.UserId == user.Id).Select(p => p.CreatedOn).ToListAsync(cancellationToken);
        var mostRecentHistory = createdOns.OrderByDescending(p => p).FirstOrDefault();
        int modifiedCount = createdOns.Count;

        var timePassed = mostRecentHistory.AddHours(24) - DateTime.UtcNow;
        var timeRemaining = TimeSpan.FromHours(timePassed.TotalHours);
        var time = timeRemaining.ToString(@"d\:hh\:mm\:ss");
        var canUpdateUserName = timePassed.TotalHours <= 0;

        // UserNameHistory
        var hasUserNameHistory = await _context.UserNameHistoryAvailable.Where(p => p.UserId != request.UserId).AnyAsync(p => p.UserName == newUserName, cancellationToken);
        if (hasUserNameHistory)
        {
            res.SetError(M107);
            return res;
        }
        #endregion

        var userNameHistory = await _context.UserNameHistories.FirstOrDefaultAsync(p => p.UserName == newUserName);
        if (userNameHistory == null)
        {
            if (!user.IsPremium && newUserName.Length < Common.SeedWork.Constants.Validator.UserNameFree.Min)
            {
                res.SetError(M124);
                return res;
            }

            userNameHistory = new UserNameHistory
            {
                UserId = user.Id,
                UserName = newUserName,
                CreatedBy = user.Id
            };

            await _context.UserNameHistories.AddAsync(userNameHistory);
        }

        if (modifiedCount > 1)
        {
            res.SetError(M128);
            if (!canUpdateUserName)
            {
                res.SetError($"{M129} {time} or {M128}");
            }
            return res;
        }

        if (userNameHistory.UserId != user.Id)
        {
            res.SetError(M125);
            return res;
        }

        user.UserName = newUserName;
        user.NormalizedUserName = newUserName.ToUpper();

        await _context.SaveChangesAsync(default);
        res.SetSuccess(newUserName);

        return res;
    }

    #endregion
}
