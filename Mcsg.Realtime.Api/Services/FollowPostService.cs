using Microsoft.EntityFrameworkCore;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Enums;
using Common.Domain;
using Interfaces;
using Requests;

public partial class FollowPostService : IFollowPostService
{
    private readonly IMcsgContext _context;
    private readonly INotificationService _notificationService;

    public FollowPostService(

        IMcsgContext context,
        INotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    public async Task SendPostFollowNotification(FollowPostReq req)
    {
        var actor = await _context.UserAvailable.Where(p => p.Id == req.ActorId).Select(p => new { ProfileName = p.ProfileName, UserAvatar = p.Avatar }).FirstOrDefaultAsync();
        try
        {
            var postName = req.MicroService switch
            {
                nameof(MicroService.Comic) => await _context.ComicPostAvailable.Where(p => p.Id == req.PostId).Select(p => p.Title).FirstOrDefaultAsync(),
                nameof(MicroService.Story) => await _context.StoryPostAvailable.Where(p => p.Id == req.PostId).Select(p => p.Title).FirstOrDefaultAsync(),
            };

            var request = new FollowPostNotificationReq()
            {
                PostId = req.PostId,
                ActorId = req.ActorId,
                ReceiverId = req.AuthorId,
                ActorName = actor.ProfileName,
                PostHashId = req.PostHashId,
                UserAvatar = actor.UserAvatar,
                PostName = postName,
                NotificationEntityType = req.MicroService == nameof(MicroService.Comic) ? NotificationEntityType.ComicPostFollow : NotificationEntityType.StoryPostFollow,

            };
            await _notificationService.AddFollowPostNotification(request);
        }
        catch (Exception ex)
        {

        }
    }

}
