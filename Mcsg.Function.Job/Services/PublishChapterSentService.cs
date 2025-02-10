using Microsoft.EntityFrameworkCore;

namespace Mcsg.Function.Job.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Interfaces;

/// <summary>
/// PublishChapterSent service
/// </summary>
public class PublishChapterSentService : IPublishChapterSentService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    public PublishChapterSentService(IMcsgContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    /// <summary>
    /// Run
    /// </summary>
    /// <returns>Return the result</returns>
    public async Task Run()
    {
        try
        {
            var toDate = DateTime.UtcNow;
            var frDate = toDate.StartOfDayUtc();

            await ComicUpdatePublishChapterSent(frDate, toDate);
            await DocumentUpdatePublishChapterSent(frDate, toDate);
            await StoryUpdatePublishChapterSent(frDate, toDate);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }
    }

    private async Task ComicUpdatePublishChapterSent(DateTime frDate, DateTime toDate)
    {
        var subPosts = await _context.Available<ComicSubPost>(false)
             .Where(p => p.IsPublishChapterSent != true
                 && frDate <= p.PublishDate && p.PublishDate <= toDate
                 && p.Permission != PostPermission.Private)
             .Select(p => new ComicSubPost
             {
                 Id = p.Id,
                 PostId = p.PostId,
                 Order = p.Order,
                 UserId = p.UserId,
                 IsPublishChapterSent = p.IsPublishChapterSent,
             })
             .ToListAsync();

        var postIds = subPosts.Select(p => p.PostId).Distinct().ToList();

        var postDicts = (await _context.Available<ComicPost>(false)
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new ComicPost
            {
                Id = p.Id,
                HashId = p.HashId,
                Title = p.Title,
                ThumbnailUrl = p.ThumbnailUrl,
                Type = p.Type
            })
            .ToListAsync())
            .ToDictionary(p => p.Id, p => p);

        var subPostIdUpdates = new List<Guid>();

        foreach (var i in subPosts)
        {
            bool isPremium = i.Permission == PostPermission.Premium;

            if (postDicts.TryGetValue(i.PostId, out var post))
            {
                bool isSent = await SendAddSubPostNotificationAsync(i, post, isPremium);

                if (isSent)
                {
                    subPostIdUpdates.Add(i.Id);
                }
            }
        }

        if (subPostIdUpdates.Count > 0)
        {
            await _context.ComicSubPosts
                .Where(p => subPostIdUpdates.Contains(p.Id))
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsPublishChapterSent, true));
        }
    }

    private async Task DocumentUpdatePublishChapterSent(DateTime frDate, DateTime toDate)
    {
        var subPosts = await _context.Available<DocumentSubPost>(false)
             .Where(p => p.IsPublishChapterSent != true
                 && frDate <= p.PublishDate && p.PublishDate <= toDate
                 && p.Permission != PostPermission.Private)
             .Select(p => new DocumentSubPost
             {
                 Id = p.Id,
                 PostId = p.PostId,
                 Order = p.Order,
                 UserId = p.UserId,
                 IsPublishChapterSent = p.IsPublishChapterSent,
             })
             .ToListAsync();

        var postIds = subPosts.Select(p => p.PostId).Distinct().ToList();

        var postDicts = (await _context.Available<DocumentPost>(false)
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new DocumentPost
            {
                Id = p.Id,
                HashId = p.HashId,
                Title = p.Title,
                ThumbnailUrl = p.ThumbnailUrl,
                Type = p.Type
            })
            .ToListAsync())
            .ToDictionary(p => p.Id, p => p);

        var subPostIdUpdates = new List<Guid>();

        foreach (var i in subPosts)
        {
            bool isPremium = i.Permission == PostPermission.Premium;

            if (postDicts.TryGetValue(i.PostId, out var post))
            {
                bool isSent = await SendAddSubPostNotificationAsync(i, post, isPremium);

                if (isSent)
                {
                    subPostIdUpdates.Add(i.Id);
                }
            }
        }

        if (subPostIdUpdates.Count > 0)
        {
            await _context.DocumentSubPosts
                .Where(p => subPostIdUpdates.Contains(p.Id))
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsPublishChapterSent, true));
        }
    }

    private async Task StoryUpdatePublishChapterSent(DateTime frDate, DateTime toDate)
    {
        var subPosts = await _context.Available<StorySubPost>(false)
             .Where(p => p.IsPublishChapterSent != true
                 && frDate <= p.PublishDate && p.PublishDate <= toDate
                 && p.Permission != PostPermission.Private)
             .Select(p => new StorySubPost
             {
                 Id = p.Id,
                 PostId = p.PostId,
                 Order = p.Order,
                 UserId = p.UserId,
                 IsPublishChapterSent = p.IsPublishChapterSent,
             })
             .ToListAsync();

        var postIds = subPosts.Select(p => p.PostId).Distinct().ToList();

        var postDicts = (await _context.Available<StoryPost>(false)
            .Where(p => postIds.Contains(p.Id))
            .Select(p => new StoryPost
            {
                Id = p.Id,
                HashId = p.HashId,
                Title = p.Title,
                ThumbnailUrl = p.ThumbnailUrl,
                Type = p.Type
            })
            .ToListAsync())
            .ToDictionary(p => p.Id, p => p);

        var subPostIdUpdates = new List<Guid>();

        foreach (var i in subPosts)
        {
            bool isPremium = i.Permission == PostPermission.Premium;

            if (postDicts.TryGetValue(i.PostId, out var post))
            {
                bool isSent = await SendAddSubPostNotificationAsync(i, post, isPremium);

                if (isSent)
                {
                    subPostIdUpdates.Add(i.Id);
                }
            }
        }

        if (subPostIdUpdates.Count > 0)
        {
            await _context.StorySubPosts
                .Where(p => subPostIdUpdates.Contains(p.Id))
                .ExecuteUpdateAsync(p => p.SetProperty(p => p.IsPublishChapterSent, true));
        }
    }

    private async Task<bool> SendAddSubPostNotificationAsync(BaseSubPost subPost, BasePost post, bool isPremium)
    {
        var followerUserIds = await _context.Available<NotificationObject>(false)
                .Where(p => p.LocationId == subPost.PostId && p.Action == NotificationAction.FollowPost)
                .Select(p => p.ActorId)
                .Distinct()
                .ToListAsync();

        if (isPremium && followerUserIds.Count > 0)
        {
            followerUserIds = await _context.UserAvailable
                .Where(p => p.IsPremium && followerUserIds.Contains(p.Id))
                .Select(p => p.Id)
                .ToListAsync();
        }

        if (followerUserIds.Count <= 0)
        {
            return false;
        }

        var notiReq = new NotificationAddSubPostR
        {
            FollowerUserIds = followerUserIds,
            PostType = post.Type,
            AuthorId = subPost.UserId,
            PostId = subPost.PostId,
            PostName = post.Title,
            SubPostId = subPost.Id,
            PostHashId = post.HashId,
            PostThumbnailUrl = post.ThumbnailUrl,
            Order = subPost.Order
        };

        await AddSubPosttionNotificationAsync(notiReq);

        return true;
    }

    private async Task<bool> AddSubPosttionNotificationAsync(NotificationAddSubPostR req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/AddSubPost");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();

            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
