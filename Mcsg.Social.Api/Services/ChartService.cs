using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
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
        var qPost = _context.Available<SocialPost>().Where(p => p.UserId == userId);

        #region -- Comment --
        var commentCounts = await qPost
            .SelectMany(p => p.SocialPostComments)
            .Where(q => !q.IsDelete && q.CreatedOn >= lastDayToGetData)
            .GroupBy(p => p.CreatedOn.Date)
            .Select(g => new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
            })
            .ToListAsync();


        var subCommentCounts = await qPost
            .SelectMany(p => p.SocialSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.SocialSubPostComments)
            .Where(x => !x.IsDelete && x.CreatedOn >= lastDayToGetData)
            .GroupBy(x => x.CreatedOn.Date)
            .Select(g => new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
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
        var postReactionCounts = await qPost
            .SelectMany(p => p.SocialPostReactions)
            .Where(q => !q.IsDelete && q.CreatedOn >= lastDayToGetData)
            .GroupBy(p => p.CreatedOn.Date)
            .Select(g => new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
            })
            .ToListAsync();

        var subPostReactionCounts = await qPost
            .SelectMany(p => p.SocialSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.SocialSubPostReactions)
            .Where(q => !q.IsDelete && q.CreatedOn >= lastDayToGetData)
            .GroupBy(q => q.CreatedOn.Date)
            .Select(g => new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
            })
            .ToListAsync();

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

        var interactions = await GetNumberOfInteractionProfile(userId, lastDayToCompare, lastDayToGetData);

        return new FeedChartResponse
        {
            ChartResponseComment = totalComment,
            ChartResponseReact = totalReaction,
            CommentInteractions = interactions.CommentInteractions,
            ReactionInteractions = interactions.ReactionInteractions,
            PostInteractions = interactions.PostInteractions
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

        result.PostCount = await _context.Available<SocialPost>().CountAsync(p => p.CreatedBy == userId);
        result.ComicCount = await _context.Available<ComicPost>().CountAsync(p => p.CreatedBy == userId);
        result.StoryCount = await _context.Available<StoryPost>().CountAsync(p => p.CreatedBy == userId);

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
            var comicPostCommentLast14Days = from post in _context.Available<ComicPost>()
                                             join postComment in _context.Available<ComicPostComment>()
                                             on post.Id equals postComment.PostId
                                             where post.UserId == userId
                                             && postComment.CreatedOn >= lastDayToCompare
                                             select postComment;

            var comicReactionsLast14Days = from post in _context.Available<ComicPost>()
                                           join postReaction in _context.Available<ComicPostReaction>()
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
            var comicPostCommentLast14Days = from post in _context.Available<StoryPost>()
                                             join postComment in _context.Available<StoryPostComment>()
                                             on post.Id equals postComment.PostId
                                             where post.UserId == userId
                                             && postComment.CreatedOn >= lastDayToCompare
                                             select postComment;

            var comicReactionsLast14Days = from post in _context.Available<StoryPost>()
                                           join postReaction in _context.Available<StoryPostReaction>()
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
            query = from comment in _context.Available<ComicPostReaction>()
                    join post in _context.Available<ComicPost>()
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
            query = from comment in _context.Available<StoryPostReaction>()
                    join post in _context.Available<StoryPost>()
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
            query = from comment in _context.Available<ComicPostComment>()
                    join post in _context.Available<ComicPost>()
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
            query = from comment in _context.Available<StoryPostComment>()
                    join post in _context.Available<StoryPost>()
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
        var lastDayToGetData = today.AddDays(-days);
        var lastDayToCompare = today.AddDays(-days * 2);

        var userFollowingIds = await _context.Available<UserFollow>()
            .Where(p => p.UserFollowerId == userId)
            .Select(p => p.UserFollowingId)
            .ToListAsync();

        var userFollowingThisUserForChart = _context.Available<UserFollow>()
            .Where(p => p.UserFollowingId == userId && p.CreatedOn >= lastDayToGetData)
            .GroupBy(p => p.CreatedOn.Date)
            .Select(g => new ChartResponse
            {
                Label = new DateTime(g.Key.Year, g.Key.Month, g.Key.Day).ToLabel("dd MMMM"),
                Quantity = g.Count()
            })
            .ToList();

        var userFollowing = from a in _context.UserAvailable
                            join b in _context.Available<UserFollow>()
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

        return new FollowersChartResponse
        {
            UserFollowedResponses = await userFollowing.ToListAsync(),
            ChartResponse = MapChartData(days, timezoneOffset, userFollowingThisUserForChart),
            FollowerInteractions = await GetFollowerInteractionsAsync(userId, lastDayToCompare, lastDayToGetData)
        };
    }

    private async Task<FeedChartResponse> GetNumberOfInteractionProfile(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var result = new FeedChartResponse();
        var qPost = _context.Available<SocialPost>().Where(p => p.UserId == userId);

        #region -- Comment --
        var qPostComment = qPost.SelectMany(p => p.SocialPostComments).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostComment = qPost.SelectMany(p => p.SocialSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.SocialSubPostComments)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var countPostComment = await qPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostComment = await qSubPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalComment = countPostComment + countSubPostComment;

        var countCommentPostBefore = await qPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var countCommentSubPostBefore = await qSubPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalCommentBefore = countCommentPostBefore + countCommentSubPostBefore;
        #endregion

        #region -- Reaction --
        var qPostReaction = qPost.SelectMany(p => p.SocialPostReactions).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostReaction = qPost.SelectMany(p => p.SocialSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.SocialSubPostReactions)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var countPostReaction = await qPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostReaction = await qSubPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalReaction = countPostReaction + countSubPostReaction;

        var countPostReactionBefore = await qPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var countSubPostReactionBefore = await qSubPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalReactionBefore = countPostReactionBefore + countSubPostReactionBefore;
        #endregion

        #region -- Post --
        var totalData = totalComment + totalReaction;
        var totalCompare = totalCommentBefore + totalReactionBefore;
        #endregion

        result.CommentInteractions = await GetInteractions(totalComment, totalCommentBefore);
        result.ReactionInteractions = await GetInteractions(totalReaction, totalReactionBefore);
        result.PostInteractions = await GetInteractions(totalData, totalCompare);

        return result;
    }

    private List<ChartResponse> MapChartData(int days, int timezoneOffset, List<ChartResponse> data)
    {
        var result = new List<ChartResponse>();

        if (data.Count == 0)
        {
            return result;
        }

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
        result.Reverse();
        return result;
    }

    private async Task<Interactions> GetFollowerInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var userFollowing = from a in _context.Available<UserFollow>()
                            where a.UserFollowingId == userId &&
                            a.CreatedOn >= dateToCompare
                            select a;

        var userFollowingToShow = await userFollowing.CountAsync(p => p.CreatedOn >= dateToGetData);
        var userFollowingToCompare = await userFollowing.CountAsync(p => p.CreatedOn < dateToGetData);

        return await GetInteractions(userFollowingToShow, userFollowingToCompare);
    }

    private async Task<Reaction> GetComicInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var q = _context.Available<ComicPost>().Where(p => p.UserId == userId);

        var qPostComment = q.SelectMany(p => p.ComicPostComments).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostComment = q.SelectMany(p => p.ComicSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.ComicSubPostComments)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var qPostReaction = q.SelectMany(p => p.ComicPostReactions).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostReaction = q.SelectMany(p => p.ComicSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.ComicSubPostReactions)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var countPostComment = await qPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostComment = await qSubPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalComment = countPostComment + countSubPostComment;

        var countCommentPostBefore = await qPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var countCommentSubPostBefore = await qSubPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalCommentBefore = countCommentPostBefore + countCommentSubPostBefore;

        var countPostReaction = await qPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostReaction = await qSubPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalReaction = countPostReaction + countSubPostReaction;

        var countPostReactionBefore = await qPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var countSubPostReactionBefore = await qSubPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalReactionBefore = countPostReactionBefore + countSubPostReactionBefore;

        return new Reaction(totalComment + totalReaction, totalCommentBefore + totalReactionBefore);
    }

    private async Task<Interactions> GetSocialInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var q = _context.Available<SocialPost>().Where(p => p.UserId == userId);

        var qPostComment = q.SelectMany(p => p.SocialPostComments).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostComment = q.SelectMany(p => p.SocialSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.SocialSubPostComments)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var qPostReaction = q.SelectMany(p => p.SocialPostReactions).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostReaction = q.SelectMany(p => p.SocialSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.SocialSubPostReactions)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var countPostComment = await qPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostComment = await qSubPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalComment = countPostComment + countSubPostComment;

        var countCommentPostBefore = await qPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var countCommentSubPostBefore = await qSubPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalCommentBefore = countCommentPostBefore + countCommentSubPostBefore;

        var countPostReaction = await qPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostReaction = await qSubPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalReaction = countPostReaction + countSubPostReaction;

        var countPostReactionBefore = await qPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var countSubPostReactionBefore = await qSubPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalReactionBefore = countPostReactionBefore + countSubPostReactionBefore;

        return await GetInteractions(totalComment + totalReaction, totalCommentBefore + totalReactionBefore);
    }

    private async Task<Reaction> GetStoryInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var q = _context.Available<StoryPost>().Where(p => p.UserId == userId);

        var qPostComment = q.SelectMany(p => p.StoryPostComments).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostComment = q.SelectMany(p => p.StorySubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.StorySubPostComments)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var qPostReaction = q.SelectMany(p => p.StoryPostReactions).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostReaction = q.SelectMany(p => p.StorySubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.StorySubPostReactions)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var countPostComment = await qPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostComment = await qSubPostComment.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalComment = countPostComment + countSubPostComment;

        var countCommentPostBefore = await qPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var countCommentSubPostBefore = await qSubPostComment.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalCommentBefore = countCommentPostBefore + countCommentSubPostBefore;

        var countPostReaction = await qPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var countSubPostReaction = await qSubPostReaction.CountAsync(p => p.CreatedOn >= dateToGetData);
        var totalReaction = countPostReaction + countSubPostReaction;

        var countPostReactionBefore = await qPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var countSubPostReactionBefore = await qSubPostReaction.CountAsync(p => p.CreatedOn < dateToGetData);
        var totalReactionBefore = countPostReactionBefore + countSubPostReactionBefore;

        return new Reaction(totalComment + totalReaction, totalCommentBefore + totalReactionBefore);
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
