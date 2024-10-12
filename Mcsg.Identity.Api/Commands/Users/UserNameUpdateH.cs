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
using Dtos;
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
            res.SetError(E125, M125);
            return res;
        }

        if (!request.IsAdministrator && CheckUsernameIsReserved(newUserName))
        {
            res.SetError(E107, M107);
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
        var hasUserNameHistory = await _context.UserNameHistoryAvailable.AnyAsync(p => p.UserName == newUserName, cancellationToken);
        if (hasUserNameHistory)
        {
            res.SetError(E107, M107);
            return res;
        }

        var dto = await Validate(user.Id, cancellationToken);

        if (dto.TimePassed.TotalMinutes < dto.UserNameWaitingChangedAfter)
        {
            if ((dto.ModifiedCount == 2 && !dto.CanUpdateUserName) || ((!dto.CanUpdateUserName || dto.UpdatedUserName) && dto.ModifiedCount > 2))
            {
                res.SetError(E128, M128 + dto.TimeWaiting);
                return res;
            }
        }
        #endregion

        var userNameHistory = await _context.UserNameHistories.FirstOrDefaultAsync(p => p.UserName == newUserName, cancellationToken);
        if (userNameHistory != null)
        {
            await _context.UserNameHistories
            .Where(p => p.UserName == newUserName)
            .OrderByDescending(p => p.CreatedOn)
            .Take(1)
            .ExecuteUpdateAsync(p => p.SetProperty(q => q.IsDelete, true), cancellationToken);
        }

        userNameHistory = new UserNameHistory
        {
            UserId = user.Id,
            UserName = newUserName,
            CreatedBy = user.Id
        };

        await _context.UserNameHistories.AddAsync(userNameHistory, cancellationToken);
        user.UserName = newUserName;
        user.NormalizedUserName = newUserName.ToUpper();

        await _context.SaveChangesAsync(cancellationToken);

        dto = await Validate(user.Id, cancellationToken);

        string m100 = $"{S100}. You have {dto.TimeRemaining} left to edit username again";
        string m101 = $"{S101}. You need to wait {dto.TimeWaiting} to edit username";

        if (dto.ModifiedCount <= 2)
        {
            dto.UpdatedUserName = false;
        }

        var data = new
        {
            code = dto.UpdatedUserName ? nameof(S101) : nameof(S100),
            message = dto.UpdatedUserName ? m101 : m100,
            userName = newUserName,
            waitingTime = dto.TimeWaiting,
            remainingTime = dto.TimeRemaining
        };

        res.SetSuccess(data);
        return res;
    }

    /// <summary>
    /// Validate the user's ability to change their username
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns the validation result as a <see cref="ChangeUserNameValidatorDto"/></returns>
    private async Task<ChangeUserNameValidatorDto> Validate(Guid userId, CancellationToken cancellationToken)
    {
        var userNameWaitingChangedAfter = _setting.UserNameWaitingChangedAfter;
        var userNameChangedInRemaining = _setting.UserNameChangedInRemaining;

        var createdOns = await _context.UserNameHistoryAvailable
            .Where(p => p.UserId == userId)
            .Select(p => p.CreatedOn)
            .ToListAsync(cancellationToken);

        var latestChanged = createdOns.OrderByDescending(p => p).FirstOrDefault();
        var previousChanged = createdOns.OrderByDescending(p => p).Skip(1).FirstOrDefault();
        var modifiedCount = createdOns.Count;

        var utcNow = DateTime.UtcNow;
        var remainingTime = utcNow - latestChanged.AddMinutes(userNameChangedInRemaining);
        var waitTime = latestChanged.AddMinutes(userNameWaitingChangedAfter);
        var timePassed = utcNow - latestChanged;
        var timePreviousPassed = utcNow - previousChanged.AddMinutes(userNameChangedInRemaining);

        var canUpdateUserName = remainingTime.TotalMinutes <= 0;
        var updatedUserName = (timePreviousPassed.TotalMinutes - remainingTime.TotalMinutes) <= userNameChangedInRemaining;
        var timeRemaining = remainingTime.ToString(@"hh\:mm\:ss");

        var result = new ChangeUserNameValidatorDto
        {
            CanUpdateUserName = canUpdateUserName,
            UpdatedUserName = updatedUserName,
            TimeRemaining = timeRemaining,
            TimeWaiting = waitTime,
            ModifiedCount = modifiedCount,
            UserNameWaitingChangedAfter = userNameWaitingChangedAfter,
            UserNameChangedInRemaining = userNameChangedInRemaining,
            TimePassed = timePassed
        };
        return result;
    }

    /// <summary>
    /// Checks if the username is reserved.
    /// </summary>
    /// <param name="userName">The username to check.</param>
    /// <returns>True if the username is reserved, otherwise false.</returns>
    private bool CheckUsernameIsReserved(string userName)
    {
        // Split the reserved usernames from settings
        var reservedUsernames = _setting.UsernameIsReserved.Split(';', StringSplitOptions.RemoveEmptyEntries);

        // Check if the provided username contains any reserved usernames
        foreach (var reserved in reservedUsernames)
        {
            if (userName.Contains(reserved, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}
