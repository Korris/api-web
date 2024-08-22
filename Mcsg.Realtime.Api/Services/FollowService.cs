using Microsoft.EntityFrameworkCore;

namespace Mcsg.Realtime.Api.Services;

using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Interfaces;
using Lib.Common.Web.Security;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class FollowService : IFollowService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IMcsgContext _context;
    private readonly INotificationService _notificationService;

    public FollowService(ICurrentUserService currentUserService, IMcsgContext context, INotificationService notificationService)
    {
        _currentUserService = currentUserService;
        _context = context;
        _notificationService = notificationService;
    }

    public async Task<UserFollowResp> FollowUser(FollowUserR request)
    {
        if (request.FollowedId == Guid.Empty)
        {
            throw new BadRequestException(E119, M119);
        }

        var user = await _context.UserAvailable.FirstOrDefaultAsync(p => p.Id == request.CreatedByUserId);
        if (user == null)
        {
            throw new BadRequestException(E119, M119);
        }
        if (request.CreatedByUserId == request.FollowedId)
        {
            throw new BadRequestException(E120, M120);
        }

        var qUserFollow = _context.UserFollows.Where(p => p.UserFollowerId == request.CreatedByUserId && p.UserFollowingId == request.FollowedId);
        var userFollow = await qUserFollow.FirstOrDefaultAsync();
        if (userFollow != null)
        {
            if (!userFollow.IsDelete)
            {
                userFollow.IsDelete = true;
                userFollow.ModifiedOn = DateTime.UtcNow;
                userFollow.ModifiedBy = request.CreatedByUserId;
                _context.UserFollows.Update(userFollow);
                await _context.SaveChangesAsync(default);
                var UserFollowResp = new UserFollowResp
                {
                    CreatedByUserId = userFollow.UserFollowerId,
                    FollowedId = userFollow.UserFollowingId,
                    CreatedOn = userFollow.CreatedOn,
                    Status = !userFollow.IsDelete,
                    CreatedByUserName = user.UserName + "",
                    CreatedByUserAvata = user.Avatar + "",
                };

                await _context.SaveChangesAsync(default);

                return UserFollowResp;
            }
            else
            {
                userFollow.IsDelete = false;
                userFollow.ModifiedOn = DateTime.UtcNow;
                userFollow.ModifiedBy = request.CreatedByUserId;
                _context.UserFollows.Update(userFollow);
                await _context.SaveChangesAsync(default);
                var UserFollowResp = new UserFollowResp
                {
                    CreatedByUserId = userFollow.UserFollowerId,
                    FollowedId = userFollow.UserFollowingId,
                    CreatedOn = userFollow.CreatedOn,
                    Status = !userFollow.IsDelete,
                    CreatedByUserName = user.UserName + "",
                    CreatedByUserAvata = user.Avatar + "",
                };

                await _notificationService.FollowNotification(UserFollowResp);
                await _context.SaveChangesAsync(default);

                return UserFollowResp;
            }
        }
        userFollow = new UserFollow
        {
            UserFollowerId = request.CreatedByUserId,
            UserFollowingId = request.FollowedId,
            CreatedOn = DateTime.UtcNow,
            CreatedBy = request.CreatedByUserId,
            ModifiedOn = DateTime.UtcNow,
            ModifiedBy = request.CreatedByUserId,
            IsDelete = false
        };

        await _context.UserFollows.AddAsync(userFollow);

        var UserFollowNewResp = new UserFollowResp
        {
            CreatedByUserId = userFollow.UserFollowerId,
            FollowedId = userFollow.UserFollowingId,
            CreatedOn = userFollow.CreatedOn,
            Status = !userFollow.IsDelete,
            CreatedByUserName = user.UserName + "",
            CreatedByUserAvata = user.Avatar + "",
        };

        await _notificationService.FollowNotification(UserFollowNewResp);
        await _context.SaveChangesAsync(default);

        return UserFollowNewResp;
    }

}
