using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;

public partial class FollowPostService : IFollowPostService
{
    private readonly IMcsgContext _context;
    private readonly INotificationService _notificationService;

    public FollowPostService(IMcsgContext context, INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task SendPostFollowNotification(FollowPostReq req)
    {
        var actor = await _context.UserAvailable
            .Where(p => p.Id == req.ActorId)
            .Select(p => new { p.ProfileName, UserAvatar = p.Avatar })
            .FirstOrDefaultAsync();

        if (actor == null) return;

        try
        {
            var microService = req.MicroService.ToEnum(MicroService.Social);
            var qTitle = microService switch
            {
                MicroService.Comic => _context.Available<ComicPost>().Where(p => p.Id == req.PostId).Select(p => p.Title),
                MicroService.Document => _context.Available<DocumentPost>().Where(p => p.Id == req.PostId).Select(p => p.Title),
                _ => _context.Available<StoryPost>().Where(p => p.Id == req.PostId).Select(p => p.Title),
            };
            var title = await qTitle.FirstOrDefaultAsync();

            var notiEntityType = microService switch
            {
                MicroService.Comic => NotificationEntityType.ComicPostFollow,
                MicroService.Document => NotificationEntityType.DocumentPostFollow,
                _ => NotificationEntityType.StoryPostFollow,
            };

            var request = new FollowPostNotificationReq
            {
                PostId = req.PostId,
                ActorId = req.ActorId,
                ReceiverId = req.AuthorId,
                ActorName = actor.ProfileName + "",
                PostHashId = req.PostHashId,
                UserAvatar = actor.UserAvatar + "",
                PostName = title + "",
                NotificationEntityType = notiEntityType
            };

            await _notificationService.AddFollowPostNotification(request);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }
}
