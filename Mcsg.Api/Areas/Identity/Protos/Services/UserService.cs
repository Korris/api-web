using Grpc.Core;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Identity.Protos.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;

public class UserService : UserProto.UserProtoBase
{
    #region -- Overrides --

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserUpdateRsp> Update(UserUpdateReq request, ServerCallContext context)
    {
        var res = new UserUpdateRsp();

        try
        {
            var id = new Guid(request.UserUid);
            var ett = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == id);
            if (ett != null)
            {
                ett.PremiumDate = request.PremiumDate.ToDateTime();
                ett.NextCheckPremium = null;
                ett.IsExpiredSubscriptionSent = null;

                res.Success = await _context.SaveChangesAsync(context.CancellationToken) > 0;
                res.Id = ett.Id.ToString();
            }
            else
            {
                res.Message = "User not found.";
            }
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    /// <summary>
    /// Search
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserSearchRsp> Search(UserSearchReq request, ServerCallContext context)
    {
        var res = new UserSearchRsp();

        try
        {
            var id = request.UserUid.Split(";");
            var listUsers = await _context.UserAvailable.Where(p => id.Contains(p.Id.ToString()))
                .Select(p => new UserProtoDto
                {
                    UserId = p.Id.ToString(),
                    UserName = p.UserName,
                    ProfileName = p.ProfileName,
                    UserAvatar = p.Avatar + "",
                    Email = p.Email + "",
                    PhoneNumber = p.PhoneNumber + ""
                }).ToListAsync();
            res.Items.AddRange(listUsers);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    /// <summary>
    /// Reset
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="context">Context</param>
    /// <returns>Return the result</returns>
    public override async Task<UserPremiumResetRsp> Reset(UserPremiumResetReq request, ServerCallContext context)
    {
        var res = new UserPremiumResetRsp();
        try
        {
            var userIds = request.UserIds.Select(p => Guid.Parse(p)).ToList();
            if (userIds.Count <= 0)
            {
                res.Message = "UserIds not empty";
                return res;
            }

            var users = await _context.Users.Where(p => userIds.Contains(p.Id)).ToListAsync(context.CancellationToken);
            if (users == null)
            {
                res.Message = "User not found.";
                return res;
            }

            // Reset PremiumDate and delete transaction notifications
            users.ForEach(p => p.Reset());

            var entityTypes = new List<NotificationEntityType>
            {
                NotificationEntityType.TransferTransaction,
                NotificationEntityType.DonateTransaction,
                NotificationEntityType.DepositTransaction,
                NotificationEntityType.BuyPremiumTransaction,
                NotificationEntityType.BuyUpgradePremiumTransaction,
                NotificationEntityType.BuyRenewPremiumTransaction,
                NotificationEntityType.RemindExpiredSubscription,
                NotificationEntityType.ExpiredSubscription
            };

            var notifications = await _context.Notifications.Include(p => p.NotificationObject)
                .Where(p => entityTypes.Contains(p.NotificationObject.EntityType) && p.ReceiverId.HasValue && userIds.Contains(p.ReceiverId.Value))
                .ToListAsync(context.CancellationToken);
            var notificationObjects = notifications.Select(p => p.NotificationObject).ToList();

            _context.NotificationObjects.RemoveRange(notificationObjects);
            _context.Notifications.RemoveRange(notifications);

            res.Success = await _context.SaveChangesAsync(context.CancellationToken) > 0;
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }
        return res;
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    public UserService(IMcsgContext context)
    {
        _context = context;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
