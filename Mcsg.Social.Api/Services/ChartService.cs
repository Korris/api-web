using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Extensions;
using Common.Domain.Entities;
using Common.Domain.Interfaces;
using Extensions;
using Interfaces;
using Models;

public partial class ChartService : IChartService
{
    public ChartService(IMcsgContext context, ISetting setting)
    {
        _context = context;
        _setting = setting;
    }

    public async Task<FeedChartResponse> GetInteractionChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days)
    {
        var nowUtc = DateTime.Today.ToUniversalTime();
        var days = isGetDataIn7Days ? 7 : 30;
        var daysAgoUtc = nowUtc.AddDays(-days);
        DateTime lastDayToGetData = nowUtc.AddDays(-days);
        DateTime lastDayToCompare = nowUtc.AddDays(-days * 2);
        var qPost = GetSocialPostQuery(userId, daysAgoUtc, nowUtc);

        #region -- Comment --
        var commentCounts = await qPost
            .Join(_context.SocialPostCommentAvailable, post => post.Id, comment => comment.PostId, (post, comment) => new { Post = post, Comment = comment })
            .GroupBy(x => x.Post.CreatedOn.Date)
            .Select(group => new ChartResponse
            {
                Label = group.Key.ToLabel("dd MMMM"),
                Quantity = group.Count()
            })
            .ToListAsync();

        var subCommentCounts = await qPost
            .Join(_context.SocialSubPostAvailable, post => post.Id, subPost => subPost.PostId, (post, subPost) => new { Post = post, SubPost = subPost })
            .Join(_context.SocialSubPostCommentAvailable, x => x.SubPost.Id, subComment => subComment.PostId, (x, subComment) => new { x.Post, SubComment = subComment })
            .GroupBy(x => x.Post.CreatedOn.Date)
            .Select(group => new ChartResponse
            {
                Label = group.Key.ToLabel("dd MMMM"),
                Quantity = group.Count()
            })
            .ToListAsync();

        var totalComment = commentCounts.Concat(subCommentCounts)
            .GroupBy(x => x.Label)
            .Select(g => new ChartResponse
            {
                Label = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        totalComment = MapChartData(days, timezoneOffset, totalComment);
        #endregion

        #region -- Reaction --
        var postReactionCounts = await (
            from post in qPost
            join reaction in _context.SocialPostReactionAvailable on post.Id equals reaction.TargetId
            group 1 by post.CreatedOn.Date into g
            select new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
            }
        ).ToListAsync();

        var subPostReactionCounts = await (
            from post in qPost
            join subPost in _context.SocialSubPostAvailable on post.Id equals subPost.PostId
            join subPostReaction in _context.SocialSubPostReactions on subPost.Id equals subPostReaction.TargetId
            group 1 by post.CreatedOn.Date into g
            select new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
            }
        ).ToListAsync();

        var totalReaction = postReactionCounts.Concat(subPostReactionCounts)
            .GroupBy(x => x.Label)
            .Select(g => new ChartResponse
            {
                Label = g.Key,
                Quantity = g.Sum(x => x.Quantity)
            })
            .ToList();

        totalReaction = MapChartData(days, timezoneOffset, totalReaction);
        #endregion

        var interactions = await GetNumberOfInteractionProfile(userId, isGetDataIn7Days);

        return new FeedChartResponse
        {
            ChartResponseComment = totalComment,
            ChartResponseReact = totalReaction,
            CommentInteractions = interactions.CommentInteractions,
            ReactionInteractions = interactions.ReactionInteractions,
            PostInteractions = await GetSocialInteractionsAsync(userId, lastDayToCompare, lastDayToGetData)
        };
    }

    public async Task<GeneralInfoResponse> GetGeneralInfo(Guid? userId)
    {
        var result = new GeneralInfoResponse();

        var today = DateTime.Today.ToUniversalTime();
        var dateToGetData = today.AddDays(-7);
        var dateToCompare = today.AddDays(-14);

        result.Followers = await GetFollowerInteractionsAsync(userId, dateToCompare, dateToGetData);
        result.PostInteraction = await GetSocialInteractionsAsync(userId, dateToCompare, dateToGetData);
        result.ComicStoryInteraction = await GetComicStoryInteractionsAsync(userId, dateToCompare, dateToGetData);

        result.PostCount = await _context.SocialPostAvailable.CountAsync(p => p.CreatedBy == userId);
        result.ComicCount = await _context.ComicPostAvailable.CountAsync(p => p.CreatedBy == userId);
        result.StoryCount = await _context.StoryPostAvailable.CountAsync(p => p.CreatedBy == userId);

        return result;
    }

    public async Task<ComicChartResponse> GetComicOrStoryChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days, bool isComic)
    {
        var result = new ComicChartResponse();

        var days = isGetDataIn7Days ? 7 : 30;
        var today = DateTime.Today.ToUniversalTime();
        DateTime lastDayToGetData = today.AddDays(-days);
        DateTime lastDayToCompare = today.AddDays(-days * 2);

        if (isComic)
        {
            var comicPostCommentLast14Days = from post in _context.ComicPostAvailable
                                             join postComment in _context.ComicPostCommentAvailable
                                             on post.Id equals postComment.PostId
                                             where post.UserId == userId
                                             && postComment.CreatedOn >= lastDayToCompare
                                             select postComment;

            var comicReactionsLast14Days = from post in _context.ComicPostAvailable
                                           join postReaction in _context.ComicPostReactionAvailable
                                           on post.Id equals postReaction.TargetId
                                           where post.UserId == userId
                                             && postReaction.CreatedOn >= lastDayToCompare
                                           select postReaction;

            var comicPostCommentData = await comicPostCommentLast14Days.CountAsync(p => p.CreatedOn >= lastDayToGetData);
            var comicPostCommentCompare = await comicPostCommentLast14Days.CountAsync(p => p.CreatedOn < lastDayToGetData);

            result.ComicCommentInteractions = await GetInteractions(comicPostCommentData, comicPostCommentCompare);

            var comicReactionsData = await comicReactionsLast14Days.CountAsync(p => p.CreatedOn >= lastDayToGetData);
            var comicReactionCompare = await comicReactionsLast14Days.CountAsync(p => p.CreatedOn < lastDayToGetData);

            result.ComicReactionInteractions = await GetInteractions(comicReactionsData, comicReactionCompare);


            var data = comicPostCommentData + comicReactionsData;
            var dataCompare = comicPostCommentCompare + comicReactionCompare;

            result.ComicInteractions = await GetInteractions(data, dataCompare);

        }
        else
        {
            var comicPostCommentLast14Days = from post in _context.StoryPostAvailable
                                             join postComment in _context.StoryPostCommentAvailable
                                             on post.Id equals postComment.PostId
                                             where post.UserId == userId
                                             && postComment.CreatedOn >= lastDayToCompare
                                             select postComment;

            var comicReactionsLast14Days = from post in _context.StoryPostAvailable
                                           join postReaction in _context.StoryPostReactionAvailable
                                           on post.Id equals postReaction.TargetId
                                           where post.UserId == userId
                                             && postReaction.CreatedOn >= lastDayToCompare
                                           select postReaction;

            var comicPostCommentData = await comicPostCommentLast14Days.CountAsync(p => p.CreatedOn >= lastDayToGetData);
            var comicPostCommentCompare = await comicPostCommentLast14Days.CountAsync(p => p.CreatedOn < lastDayToGetData);

            result.ComicCommentInteractions = await GetInteractions(comicPostCommentData, comicPostCommentCompare);

            var comicReactionsData = await comicReactionsLast14Days.CountAsync(p => p.CreatedOn >= lastDayToGetData);
            var comicReactionCompare = await comicReactionsLast14Days.CountAsync(p => p.CreatedOn < lastDayToGetData);

            result.ComicReactionInteractions = await GetInteractions(comicReactionsData, comicReactionCompare);

            var data = comicPostCommentData + comicReactionsData;
            var dataCompare = comicPostCommentCompare + comicReactionCompare;

            result.ComicInteractions = await GetInteractions(data, dataCompare);
        }

        result.CommentChartResponse = await GetComicCommentChart(userId, timezoneOffset, days, isComic);
        result.ReactionChartResponse = await GetComicReactionChart(userId, timezoneOffset, days, isComic);
        return result;
    }

    public async Task<List<ChartResponse>> GetComicReactionChart(Guid? userId, int timezoneOffset, int days, bool isComic)
    {
        var today = DateTime.Today.ToUniversalTime();
        var date = today.AddDays(-days);
        IQueryable<ChartResponse> query;
        if (isComic)
        {
            query = from comment in _context.ComicPostReactionAvailable
                    join post in _context.ComicPostAvailable
                    on comment.TargetId equals post.Id
                    where post.CreatedBy == userId
                    && comment.CreatedOn >= date
                    group comment by comment.CreatedOn.Date into g
                    select new ChartResponse
                    {
                        Label = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    };

        }
        else
        {
            query = from comment in _context.StoryPostReactionAvailable
                    join post in _context.StoryPostAvailable
                    on comment.TargetId equals post.Id
                    where post.CreatedBy == userId
                    && comment.CreatedOn >= date
                    group comment by comment.CreatedOn.Date into g
                    select new ChartResponse
                    {
                        Label = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    };

        }
        return MapChartData(days, timezoneOffset, await query.ToListAsync());
    }

    public async Task<List<ChartResponse>> GetComicCommentChart(Guid? userId, int timezoneOffset, int days, bool isComic)
    {
        var today = DateTime.Today.ToUniversalTime();
        var date = today.AddDays(-days);
        IQueryable<ChartResponse> query;
        if (isComic)
        {
            query = from comment in _context.ComicPostCommentAvailable
                    join post in _context.ComicPostAvailable
                    on comment.PostId equals post.Id
                    where post.CreatedBy == userId
                    && comment.CreatedOn >= date
                    group comment by comment.CreatedOn.Date into g
                    select new ChartResponse
                    {
                        Label = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    };

        }
        else
        {
            query = from comment in _context.StoryPostCommentAvailable
                    join post in _context.StoryPostAvailable
                    on comment.PostId equals post.Id
                    where post.CreatedBy == userId
                    && comment.CreatedOn >= date
                    group comment by comment.CreatedOn.Date into g
                    select new ChartResponse
                    {
                        Label = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    };

        }
        return MapChartData(days, timezoneOffset, await query.ToListAsync());
    }

    public async Task<FollowersChartResponse> GetFollowersChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days)
    {

        var days = isGetDataIn7Days ? 7 : 30;
        var today = DateTime.Today.ToUniversalTime();
        DateTime lastDayToGetData = today.AddDays(-days);
        DateTime lastDayToCompare = today.AddDays(-days * 2);
        var userFollowingIds = await _context.UserFollowAvailable.AsNoTracking()
                                                                    .Where(p => p.UserFollowerId == userId)
                                                                    .Select(p => p.UserFollowingId)
                                                                    .ToListAsync();

        var userFollowingThisUserForChart = _context.UserFollowAvailable
                                                .Where(p => p.UserFollowingId == userId && p.CreatedOn >= lastDayToGetData)
                                                .GroupBy(p => p.CreatedOn.Date)
                                                .Select(g => new ChartResponse
                                                {
                                                    Label = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).ToLabel("dd MMMM"),
                                                    Quantity = g.Count()
                                                })
                                                .ToList();

        var userFollowing = from a in _context.UserAvailable
                            join b in _context.UserFollowAvailable
                              on a.Id equals b.UserFollowerId
                            where b.UserFollowingId == userId
                            && b.CreatedOn >= lastDayToGetData
                            select new UserFollowedResponse
                            {
                                UserId = a.Id,
                                ProfileName = a.ProfileName,
                                Avatar = a.Avatar,
                                UserName = a.UserName,
                                IsFollowing = userFollowingIds.Contains(a.Id)
                            };

        foreach (var i in userFollowing)
        {
            i.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(i.Avatar + "");
        }

        return new FollowersChartResponse
        {
            UserFollowedResponses = await userFollowing.ToListAsync(),
            ChartResponse = MapChartData(days, timezoneOffset, userFollowingThisUserForChart),
            FollowerInteractions = await GetFollowerInteractionsAsync(userId, lastDayToCompare, lastDayToGetData)
        };
    }

    private async Task<FeedChartResponse> GetNumberOfInteractionProfile(Guid? userId, bool isGetDataIn7Days)
    {
        var result = new FeedChartResponse();
        var nowUtc = DateTime.Today.ToUniversalTime();
        var days = isGetDataIn7Days ? 7 : 30;
        var daysAgoUtc = nowUtc.AddDays(-days);
        var qPost = GetSocialPostQuery(userId, daysAgoUtc, nowUtc);

        int previousDays = days == 7 ? 7 : 30;
        var previousPeriodStart = nowUtc.AddDays(-days - previousDays); // Start of the previous period
        var previousPeriodEnd = nowUtc.AddDays(-days);
        var qPostPreviousPeriod = GetSocialPostQuery(userId, previousPeriodStart, previousPeriodEnd);

        #region -- Total Comment --
        var countCommentPost = await (
            from posts in qPost
            join comments in _context.SocialPostComments on posts.Id equals comments.PostId
            select 1
            )
            .CountAsync();

        var countCommentSubPost = await (
            from posts in qPost
            join subPosts in _context.SocialSubPostAvailable on posts.Id equals subPosts.PostId
            join subPostComments in _context.SocialSubPostCommentAvailable on subPosts.Id equals subPostComments.PostId
            select 1
            )
            .CountAsync();

        var totalComment = countCommentPost + countCommentSubPost;

        #region -- Comment in the previous 7-day period --
        var countCommentPostBefore = await (
            from posts in qPostPreviousPeriod
            join comments in _context.SocialPostComments on posts.Id equals comments.PostId
            select 1
        ).CountAsync();

        var countCommentSubPostBefore = await (
            from posts in qPostPreviousPeriod
            join subPosts in _context.SocialSubPostAvailable on posts.Id equals subPosts.PostId
            join subPostComments in _context.SocialSubPostCommentAvailable on subPosts.Id equals subPostComments.PostId
            select 1
        ).CountAsync();

        var totalCommentBefore = countCommentPostBefore + countCommentSubPostBefore;
        #endregion
        #endregion

        #region -- Total Reaction --
        var countReact = await (
                from posts in qPost
                join reactions in _context.SocialPostReactionAvailable on posts.Id equals reactions.TargetId
                where posts.UserId == userId && posts.CreatedOn >= daysAgoUtc && posts.CreatedOn <= nowUtc
                select 1
            )
            .CountAsync();

        var countReactSubPost = await (
                from posts in qPost
                join subPosts in _context.SocialSubPostAvailable on posts.Id equals subPosts.PostId
                join subPostReacts in _context.SocialSubPostReactions on subPosts.Id equals subPostReacts.TargetId
                select 1
                )
                .CountAsync();

        #region -- Reaction in the previous 7-day period --
        var countReactBefore = await (
                from posts in qPostPreviousPeriod
                join reactions in _context.SocialPostReactionAvailable on posts.Id equals reactions.TargetId
                select 1
            )
            .CountAsync();

        var countReactSubPostBefore = await (
                from posts in qPostPreviousPeriod
                join subPosts in _context.SocialSubPostAvailable on posts.Id equals subPosts.PostId
                join subPostReacts in _context.SocialSubPostReactions on subPosts.Id equals subPostReacts.TargetId
                select 1
            )
            .CountAsync();

        var totalReactBefore = countReactBefore + countReactSubPostBefore;
        #endregion
        var totalReaction = countReact + countReactSubPost;
        #endregion

        result.CommentInteractions = new Interactions
        {
            Count = totalComment,
            Percent = totalComment > 0 ? Math.Abs(((double)(totalCommentBefore - totalComment) / totalComment) * 100) : 0,
            IsIncrease = totalComment > totalReactBefore

        };
        result.ReactionInteractions = new Interactions
        {
            Count = totalReaction,
            Percent = totalReaction > 0 ? Math.Abs(((double)(totalReactBefore - totalReaction) / totalReaction) * 100) : 0,
            IsIncrease = totalReaction > totalReactBefore
        };

        return result;
    }

    private List<ChartResponse> MapChartData(int days, int timezoneOffset, List<ChartResponse> data)
    {
        var result = new List<ChartResponse>();
        var today = DateTime.Today.ToUniversalTime().AddMinutes(-timezoneOffset);
        for (int i = 0; i < days; i++)
        {
            DateTime date = today.AddDays(-i);
            var label = new DateTime(date.Year, date.Month, date.Day).ToLabel("dd MMMM");
            var quantity = data.FirstOrDefault(p => p.Label == label);
            result.Add(new ChartResponse
            {
                Label = label,
                Quantity = quantity?.Quantity ?? 0,
            });
        }
        return result;
    }

    private async Task<Interactions> GetFollowerInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var userFollowing = from a in _context.UserFollowAvailable
                            where a.UserFollowingId == userId &&
                            a.CreatedOn >= dateToCompare
                            select a;

        var userFollowingToShow = await userFollowing.CountAsync(p => p.CreatedOn >= dateToGetData);
        var userFollowingToCompare = await userFollowing.CountAsync(p => p.CreatedOn < dateToGetData);

        return await GetInteractions(userFollowingToShow, userFollowingToCompare);
    }

    private async Task<Reaction> GetComicInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var postCommentLast14Days = from post in _context.ComicPostAvailable
                                    join postComment in _context.ComicPostCommentAvailable
                                    on post.Id equals postComment.PostId
                                    where post.UserId == userId
                                    && postComment.CreatedOn >= dateToCompare
                                    select postComment;

        var postReactionsLast14Days = from post in _context.ComicPostAvailable
                                      join postReaction in _context.ComicPostReactionAvailable
                                      on post.Id equals postReaction.TargetId
                                      where post.UserId == userId
                                      && postReaction.CreatedOn >= dateToCompare
                                      select postReaction;

        int reactionsLast7Days = await postCommentLast14Days.CountAsync(p => p.CreatedOn >= dateToGetData)
                                    + await postReactionsLast14Days.CountAsync(p => p.CreatedOn >= dateToGetData);

        int reactionsPreviousLast7Days = await postCommentLast14Days.CountAsync(p => p.CreatedOn < dateToGetData)
                                + await postReactionsLast14Days.CountAsync(p => p.CreatedOn < dateToGetData);

        return new Reaction(reactionsLast7Days, reactionsPreviousLast7Days);
    }

    private async Task<Interactions> GetSocialInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var postCommentLast14Days = from post in _context.SocialPostAvailable
                                    join postComment in _context.SocialPostCommentAvailable
                                    on post.Id equals postComment.PostId
                                    where post.UserId == userId
                                    && postComment.CreatedOn >= dateToCompare
                                    select postComment;

        var postReactionsLast14Days = from post in _context.SocialPostAvailable
                                      join postReaction in _context.SocialPostReactionAvailable
                                      on post.Id equals postReaction.TargetId
                                      where post.UserId == userId
                                      && postReaction.CreatedOn >= dateToCompare
                                      select postReaction;

        int reactionsLast7Days = await postCommentLast14Days.CountAsync(p => p.CreatedOn >= dateToGetData)
                                    + await postReactionsLast14Days.CountAsync(p => p.CreatedOn >= dateToGetData);

        int reactionsPreviousLast7Days = await postCommentLast14Days.CountAsync(p => p.CreatedOn < dateToGetData)
                                + await postReactionsLast14Days.CountAsync(p => p.CreatedOn < dateToGetData);

        return await GetInteractions(reactionsLast7Days, reactionsPreviousLast7Days);
    }

    private async Task<Reaction> GetStoryInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var postCommentLast14Days = from post in _context.StoryPostAvailable
                                    join postComment in _context.StoryPostCommentAvailable
                                    on post.Id equals postComment.PostId
                                    where post.UserId == userId
                                    && postComment.CreatedOn >= dateToCompare
                                    select postComment;

        var postReactionsLast14Days = from post in _context.StoryPostAvailable
                                      join postReaction in _context.StoryPostReactionAvailable
                                      on post.Id equals postReaction.TargetId
                                      where post.UserId == userId
                                      && postReaction.CreatedOn >= dateToCompare
                                      select postReaction;

        int reactionsLast7Days = await postCommentLast14Days.CountAsync(p => p.CreatedOn >= dateToGetData)
                                    + await postReactionsLast14Days.CountAsync(p => p.CreatedOn >= dateToGetData);

        int reactionsPreviousLast7Days = await postCommentLast14Days.CountAsync(p => p.CreatedOn < dateToGetData)
                                + await postReactionsLast14Days.CountAsync(p => p.CreatedOn < dateToGetData);

        return new Reaction(reactionsLast7Days, reactionsPreviousLast7Days);
    }

    private async Task<Interactions> GetComicStoryInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var comic = await GetComicInteractionsAsync(userId, dateToCompare, dateToGetData);
        var story = await GetStoryInteractionsAsync(userId, dateToCompare, dateToGetData);

        var totalReactionsLast7Days = comic.Last7Days + story.Last7Days;
        var totalReactionsPreviousLast7Days = comic.PreviousLast7Days + story.PreviousLast7Days;

        return await GetInteractions(totalReactionsLast7Days, totalReactionsPreviousLast7Days);
    }

    private Task<Interactions> GetInteractions(int data, int dataToCompare)
    {
        return Task.FromResult(new Interactions
        {
            Count = data,
            Percent = dataToCompare > 0 ? Math.Abs(((double)(data - dataToCompare) / dataToCompare) * 100) : 0,
            IsIncrease = data > dataToCompare
        });
    }

    private IQueryable<SocialPost> GetSocialPostQuery(Guid? userId, DateTime fr, DateTime to)
    {
        return _context.SocialPostAvailable.Where(p => p.UserId == userId && fr <= p.CreatedOn && p.CreatedOn <= to);
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
