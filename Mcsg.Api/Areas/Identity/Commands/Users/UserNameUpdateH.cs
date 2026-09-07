using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Identity.Commands;

using Analytic.Application.Protos;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Dtos;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Dtos;
using Mcsg.Api.Areas.Identity.Interfaces;
using Mcsg.Api.Interfaces;
using Requests;
using Mcsg.Api.Areas.Identity.Validators;
using Wallet.Api.Protos;
using static Common.Core.Constants.Setting;
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
            return res.SetError(nameof(E000), E000, t);
        }

        var newUserName = request.NewUserName.Triz();
        if (request.UserName == newUserName)
        {
            return res.SetError(nameof(E125), E125);
        }

        if (!request.IsAdministrator && CheckUsernameIsReserved(newUserName))
        {
            return res.SetError(nameof(E107), E107);
        }

        #region -- Validate on server --
        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == request.UserId, cancellationToken);
        if (user == null)
        {
            var t = new List<DicDto> { new() { Key = nameof(request.UserId).ToCamelCase(), Value = request.UserId + "" } };
            return res.SetError(nameof(E002), E002, t);
        }

        // UserNameHistory
        var hasUserNameHistory = await _context.Available<UserNameHistory>().AnyAsync(p => p.UserName == newUserName, cancellationToken);
        if (hasUserNameHistory)
        {
            return res.SetError(nameof(E107), E107);
        }

        var userPremiumPackage = await UserPremiumPackageView(user.Id);
        var dto = await Validate(user.Id, request.TimezoneOffset, userPremiumPackage.StartDate, userPremiumPackage.EndDate, request.IsPremium, cancellationToken);

        if (dto.TimePassed.TotalMinutes < dto.UserNameWaitingChangedAfter)
        {
            if (!(dto.HasPremium && (dto.CanUpdateUserName || !dto.UpdatedUserName)))
            {
                if ((dto.ModifiedCount == 2 && !dto.CanUpdateUserName) || ((!dto.CanUpdateUserName || dto.UpdatedUserName) && dto.ModifiedCount > 2))
                {
                    var errorData = new
                    {
                        waitingTime = dto.TimeWaiting
                    };

                    return res.SetErrorData(nameof(E128), E128, errorData);
                }
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
            CreatedBy = user.Id,
            TagData = user.IsPremium ? TagData.IsPremium : null
        };

        await _context.UserNameHistories.AddAsync(userNameHistory, cancellationToken);
        user.UserName = newUserName;
        user.NormalizedUserName = newUserName.ToUpper();

        await _context.SaveChangesAsync(cancellationToken);

        dto = await Validate(user.Id, request.TimezoneOffset, userPremiumPackage.StartDate, userPremiumPackage.EndDate, request.IsPremium, cancellationToken);

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

        _ = Task.Run(async () => await SyncUpdateToAna(user));

        return res.SetSuccess(data);
    }

    /// <summary>
    /// Validate the user's ability to change their username
    /// </summary>
    /// <param name="userId">User ID</param>
    /// <param name="timezoneOffset">Timezone offset</param>
    /// <param name="startDate"></param>
    /// <param name="endDate"></param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Returns the validation result as a <see cref="ChangeUserNameValidatorDto"/></returns>
    private async Task<ChangeUserNameValidatorDto> Validate(Guid userId, int timezoneOffset, string startDate, string endDate, bool isPremium, CancellationToken cancellationToken)
    {
        var userNameWaitingChangedAfter = _setting.UserNameWaitingChangedAfter;
        var userNameChangedInRemaining = _setting.UserNameChangedInRemaining;

        var createdOns = await _context.Available<UserNameHistory>()
            .Where(p => p.UserId == userId)
            .Select(p => new { p.CreatedOn, p.TagData })
            .OrderByDescending(p => p.CreatedOn)
            .ToListAsync(cancellationToken);

        var latestChanged = createdOns.FirstOrDefault();
        var previousChanged = createdOns.Skip(1).FirstOrDefault();
        var modifiedCount = createdOns.Count;
        var hasPremium = false;

        var latestCreatedOnChanged = latestChanged?.CreatedOn ?? DateTime.MinValue;
        var previousCreatedOnChanged = previousChanged?.CreatedOn ?? DateTime.MinValue;

        var utcNow = DateTime.UtcNow;
        var remainingTime = utcNow - latestCreatedOnChanged.AddMinutes(userNameChangedInRemaining);
        var waitTime = latestCreatedOnChanged.AddMinutes(userNameWaitingChangedAfter);
        var timePassed = utcNow - latestCreatedOnChanged;
        var timePreviousPassed = utcNow - previousCreatedOnChanged.AddMinutes(userNameChangedInRemaining);

        var canUpdateUserName = remainingTime.TotalMinutes <= 0;
        var updatedUserName = (timePreviousPassed.TotalMinutes - remainingTime.TotalMinutes) <= userNameChangedInRemaining;

        if (isPremium && !string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
        {
            hasPremium = true;
            var lastestPremium = latestChanged?.TagData?.Contains(TagData.IsPremium) ?? false;
            var previousPremium = previousChanged?.TagData?.Contains(TagData.IsPremium) ?? false;

            var start = DateTime.Parse(startDate);
            var end = DateTime.Parse(endDate);

            canUpdateUserName = !lastestPremium && isPremium
                                || (lastestPremium && !(start <= latestCreatedOnChanged && latestCreatedOnChanged <= end));
            updatedUserName = previousPremium
                           && (timePreviousPassed.TotalMinutes - remainingTime.TotalMinutes) <= userNameChangedInRemaining
                           && (start <= previousCreatedOnChanged && previousCreatedOnChanged <= end);
        }

        var timeRemaining = remainingTime.ToString(@"hh\:mm\:ss");

        return new ChangeUserNameValidatorDto
        {
            CanUpdateUserName = canUpdateUserName,
            UpdatedUserName = updatedUserName,
            TimeRemaining = timeRemaining,
            TimeWaiting = waitTime.AddMinutes(-timezoneOffset),
            ModifiedCount = modifiedCount,
            UserNameWaitingChangedAfter = userNameWaitingChangedAfter,
            UserNameChangedInRemaining = userNameChangedInRemaining,
            TimePassed = timePassed,
            HasPremium = hasPremium
        };
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

    /// <summary>
    /// SyncUpdateToAna
    /// </summary>
    /// <param name="ett"></param>
    /// <returns></returns>
    private async Task<UserUpdateRsp> SyncUpdateToAna(User ett)
    {
        var res = new UserUpdateRsp() { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new UserProto.UserProtoClient(channel);

            var request = new UserUpdateReq
            {
                UserId = ett.Id.ToString(),
                Username = ett.UserName,
                ModifiedOn = ett.ModifiedOn == null ? null : ett.ModifiedOn.ToString(),
                ModifiedBy = ett.ModifiedBy == null ? null : ett.ModifiedBy.ToString()
            };
            var rsp = await client.UpdateAsync(request);

            res.Message = rsp.Message;
            res.Id = rsp.Id;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    /// <summary>
    /// UserPremiumPackageView
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    private async Task<UserPremiumPackageViewRsp> UserPremiumPackageView(Guid userId)
    {
        var res = new UserPremiumPackageViewRsp();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Wallet.Wallet!);
            var client = new UserWalletProto.UserWalletProtoClient(channel);

            var request = new UserPremiumPackageViewReq
            {
                UserId = userId.ToString(),
            };

            res = await client.UserPremiumPackageViewAsync(request);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }

        return res;
    }

    #endregion
}
