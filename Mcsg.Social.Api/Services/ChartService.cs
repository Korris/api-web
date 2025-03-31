using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Analytic.Application.Protos;
using Common.Core.Enums;
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

        #region -- Share --
        var qPostShare = await PostShareGetCount(userId, lastDayToGetData, (int)PostType.Feed);

        var totalShare = qPostShare.Items
            .GroupBy(p => DateTime.Parse(p.CreatedOn))
            .Select(g => new ChartResponse
            {
                Label = g.Key.ToLabel("dd MMMM"),
                Quantity = g.Count()
            })
            .ToList();

        totalShare = MapChartData(days, timezoneOffset, totalShare);
        #endregion

        var interactions = await GetNumberOfInteractionProfile(userId, lastDayToCompare, lastDayToGetData);

        return new FeedChartResponse
        {
            ChartResponseComment = totalComment,
            ChartResponseReact = totalReaction,
            ChartResponseShare = totalShare,
            CommentInteractions = interactions.CommentInteractions,
            ReactionInteractions = interactions.ReactionInteractions,
            ShareInteractions = interactions.ShareInteractions,
            PostInteractions = interactions.PostInteractions
        };
    }

    /// <summary>
    /// Chart information for User
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public async Task<GeneralInfoResponse> GetGeneralInfo(Guid? userId)
    {
        var result = new GeneralInfoResponse();

        var today = DateTime.Today.ToUniversalTime();
        var dateToGetData = today.AddDays(-7);
        var dateToCompare = today.AddDays(-14);

        result.Follower = await GetFollowerInteractionsAsync(userId, dateToCompare, dateToGetData);
        result.SocialInteraction = await GetSocialInteractionsAsync(userId, dateToCompare, dateToGetData);
        result.OtherInteraction = await GetComicStoryDocumentInteractionsAsync(userId, dateToCompare, dateToGetData);

        result.SocialCount = await _context.Available<SocialPost>().CountAsync(p => p.CreatedBy == userId);
        result.ComicCount = await _context.Available<ComicPost>().CountAsync(p => p.CreatedBy == userId);
        result.StoryCount = await _context.Available<StoryPost>().CountAsync(p => p.CreatedBy == userId);
        result.DocumentCount = await _context.Available<DocumentPost>().CountAsync(p => p.CreatedBy == userId);

        return result;
    }

    /// <summary>
    /// Chart information for Comic, Document and Story
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="timezoneOffset"></param>
    /// <param name="isGetDataIn7Days"></param>
    /// <param name="postType"></param>
    /// <returns></returns>
    public async Task<PostChartResponse> GetChartInfo(Guid? userId, int timezoneOffset, bool isGetDataIn7Days, PostType postType)
    {
        var result = new PostChartResponse();

        var days = isGetDataIn7Days ? 7 : 30;
        var today = DateTime.Today.ToUniversalTime();
        DateTime lastDayToGetData = today.AddDays(-days);
        DateTime lastDayToCompare = today.AddDays(-days * 2);
        var data = 0;
        var dataCompare = 0;

        switch (postType)
        {
            case PostType.Comic:
                var comic = await GetComicInteractionsAsync(userId, lastDayToCompare, lastDayToGetData);

                result.PostCommentInteractions = await GetInteractions(comic.CurrentCommentCount, comic.PreviousCommentCount);
                result.PostReactionInteractions = await GetInteractions(comic.CurrentReactionCount, comic.PreviousReactionCount);
                result.PostShareInteractions = await GetInteractions(comic.CurrentShareCount, comic.PreviousShareCount);

                data = comic.CurrentTotal;
                dataCompare = comic.PreviousTotal;
                break;

            case PostType.Story:
                var story = await GetStoryInteractionsAsync(userId, lastDayToCompare, lastDayToGetData);

                result.PostCommentInteractions = await GetInteractions(story.CurrentCommentCount, story.PreviousCommentCount);
                result.PostReactionInteractions = await GetInteractions(story.CurrentReactionCount, story.PreviousReactionCount);
                result.PostShareInteractions = await GetInteractions(story.CurrentShareCount, story.PreviousShareCount);

                data = story.CurrentTotal;
                dataCompare = story.PreviousTotal;
                break;

            default:
                var document = await GetDocumentInteractionsAsync(userId, lastDayToCompare, lastDayToGetData);

                result.PostCommentInteractions = await GetInteractions(document.CurrentCommentCount, document.PreviousCommentCount);
                result.PostReactionInteractions = await GetInteractions(document.CurrentReactionCount, document.PreviousReactionCount);
                result.PostShareInteractions = await GetInteractions(document.CurrentShareCount, document.PreviousShareCount);

                data = document.CurrentTotal;
                dataCompare = document.PreviousTotal;
                break;
        }

        result.PostInteractions = await GetInteractions(data, dataCompare);

        result.CommentChartResponse = await GetPostCommentChart(userId, timezoneOffset, days, postType);
        result.ReactionChartResponse = await GetPostReactionChart(userId, timezoneOffset, days, postType);
        result.ShareChartResponse = await GetPostShareChart(userId, timezoneOffset, days, postType);

        return result;
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

    private async Task<List<ChartResponse>> GetPostReactionChart(Guid? userId, int timezoneOffset, int days, PostType postType)
    {
        var today = DateTime.Today.ToUniversalTime();
        var date = today.AddDays(-days);
        IQueryable<ChartResponse> query;

        switch (postType)
        {
            case PostType.Comic:
                query = from comment in _context.Available<ComicPostReaction>()
                        join post in _context.Available<ComicPost>()
                        on comment.TargetId equals post.Id
                        where post.CreatedBy == userId
                        && comment.CreatedOn >= date
                        group comment by comment.CreatedOn.Date into g
                        select new ChartResponse
                        {
                            Label = g.Key.ToLabel("dd MMMM"),
                            Quantity = g.Count()
                        };
                break;

            case PostType.Story:
                query = from comment in _context.Available<StoryPostReaction>()
                        join post in _context.Available<StoryPost>()
                        on comment.TargetId equals post.Id
                        where post.CreatedBy == userId
                        && comment.CreatedOn >= date
                        group comment by comment.CreatedOn.Date into g
                        select new ChartResponse
                        {
                            Label = g.Key.ToLabel("dd MMMM"),
                            Quantity = g.Count()
                        };
                break;

            default:
                query = from comment in _context.Available<DocumentPostReaction>()
                        join post in _context.Available<DocumentPost>()
                        on comment.TargetId equals post.Id
                        where post.CreatedBy == userId
                        && comment.CreatedOn >= date
                        group comment by comment.CreatedOn.Date into g
                        select new ChartResponse
                        {
                            Label = g.Key.ToLabel("dd MMMM"),
                            Quantity = g.Count()
                        };
                break;
        }

        return MapChartData(days, timezoneOffset, await query.ToListAsync());
    }

    private async Task<List<ChartResponse>> GetPostShareChart(Guid? userId, int timezoneOffset, int days, PostType postType)
    {
        var today = DateTime.Today.ToUniversalTime();
        var date = today.AddDays(-days);
        List<ChartResponse> totalShare;

        switch (postType)
        {
            case PostType.Comic:
                var qComicShare = await PostShareGetCount(userId, date, (int)PostType.Comic);
                totalShare = qComicShare.Items
                    .GroupBy(p => DateTime.Parse(p.CreatedOn))
                    .Select(g => new ChartResponse
                    {
                        Label = g.Key.ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    })
                    .ToList();
                break;

            case PostType.Story:
                var qStoryShare = await PostShareGetCount(userId, date, (int)PostType.Story);
                totalShare = qStoryShare.Items
                    .GroupBy(p => DateTime.Parse(p.CreatedOn))
                    .Select(g => new ChartResponse
                    {
                        Label = g.Key.ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    })
                    .ToList();
                break;

            default:
                var qDocumentShare = await PostShareGetCount(userId, date, (int)PostType.Document);
                totalShare = qDocumentShare.Items
                    .GroupBy(p => DateTime.Parse(p.CreatedOn))
                    .Select(g => new ChartResponse
                    {
                        Label = g.Key.ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    })
                    .ToList();
                break;
        }

        return MapChartData(days, timezoneOffset, totalShare);
    }

    private async Task<List<ChartResponse>> GetPostCommentChart(Guid? userId, int timezoneOffset, int days, PostType postType)
    {
        var today = DateTime.Today.ToUniversalTime();
        var date = today.AddDays(-days);
        IQueryable<ChartResponse> query;

        switch (postType)
        {
            case PostType.Comic:
                var comicComments = from comment in _context.Available<ComicPostComment>()
                                    join post in _context.Available<ComicPost>()
                                    on comment.PostId equals post.Id
                                    where post.CreatedBy == userId && !comment.IsDelete && comment.CreatedOn >= date
                                    select comment;

                var subComicComments = from subComment in _context.Available<ComicSubPostComment>()
                                       join subPost in _context.Available<ComicSubPost>()
                                       on subComment.PostId equals subPost.Id
                                       join post in _context.Available<ComicPost>()
                                       on subPost.PostId equals post.Id
                                       where post.CreatedBy == userId && !subComment.IsDelete && subComment.CreatedOn >= date
                                       select subComment;

                query = comicComments.Select(c => new { c.CreatedOn })
                    .Concat(subComicComments.Select(c => new { c.CreatedOn }))
                    .GroupBy(c => c.CreatedOn.Date)
                    .Select(g => new ChartResponse
                    {
                        Label = g.Key.ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    });
                break;

            case PostType.Story:
                var storyComments = from comment in _context.Available<StoryPostComment>()
                                    join post in _context.Available<StoryPost>()
                                    on comment.PostId equals post.Id
                                    where post.CreatedBy == userId && !comment.IsDelete && comment.CreatedOn >= date
                                    select comment;

                var subStoryComments = from subComment in _context.Available<StorySubPostComment>()
                                       join subPost in _context.Available<StorySubPost>()
                                       on subComment.PostId equals subPost.Id
                                       join post in _context.Available<StoryPost>()
                                       on subPost.PostId equals post.Id
                                       where post.CreatedBy == userId && !subComment.IsDelete && subComment.CreatedOn >= date
                                       select subComment;

                query = storyComments.Select(c => new { c.CreatedOn })
                    .Concat(subStoryComments.Select(c => new { c.CreatedOn }))
                    .GroupBy(c => c.CreatedOn.Date)
                    .Select(g => new ChartResponse
                    {
                        Label = g.Key.ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    });
                break;

            default:
                var documentComments = from comment in _context.Available<DocumentPostComment>()
                                       join post in _context.Available<DocumentPost>()
                                       on comment.PostId equals post.Id
                                       where post.CreatedBy == userId && !comment.IsDelete && comment.CreatedOn >= date
                                       select comment;

                var subDocumentComments = from subComment in _context.Available<DocumentSubPostComment>()
                                          join subPost in _context.Available<DocumentSubPost>()
                                          on subComment.PostId equals subPost.Id
                                          join post in _context.Available<DocumentPost>()
                                          on subPost.PostId equals post.Id
                                          where post.CreatedBy == userId && !subComment.IsDelete && subComment.CreatedOn >= date
                                          select subComment;

                query = documentComments.Select(c => new { c.CreatedOn })
                    .Concat(subDocumentComments.Select(c => new { c.CreatedOn }))
                    .GroupBy(c => c.CreatedOn.Date)
                    .Select(g => new ChartResponse
                    {
                        Label = g.Key.ToLabel("dd MMMM"),
                        Quantity = g.Count()
                    });
                break;
        }

        return MapChartData(days, timezoneOffset, await query.ToListAsync());
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

        #region -- Share --
        var qPostShare = await PostShareGetCount(userId, dateToCompare, (int)PostType.Feed);
        var totalShare = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) >= dateToGetData);
        var totalShareBefore = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) < dateToGetData);
        #endregion

        #region -- Post --
        var totalData = totalComment + totalReaction + totalShare;
        var totalCompare = totalCommentBefore + totalReactionBefore + totalShareBefore;
        #endregion

        result.CommentInteractions = await GetInteractions(totalComment, totalCommentBefore);
        result.ReactionInteractions = await GetInteractions(totalReaction, totalReactionBefore);
        result.ShareInteractions = await GetInteractions(totalShare, totalShareBefore);
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

    private async Task<InteractionSummary> GetComicInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
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

        var qPostShare = await PostShareGetCount(userId, dateToCompare, (int)PostType.Comic);

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

        var totalSharePost = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) >= dateToGetData);
        var totalSharePostBefore = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) < dateToGetData);

        return new InteractionSummary
        {
            CurrentCommentCount = totalComment,
            PreviousCommentCount = totalCommentBefore,
            CurrentReactionCount = totalReaction,
            PreviousReactionCount = totalReactionBefore,
            CurrentShareCount = totalSharePost,
            PreviousShareCount = totalSharePostBefore
        };
    }

    private async Task<InteractionSummary> GetDocumentInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var q = _context.Available<DocumentPost>().Where(p => p.UserId == userId);

        var qPostComment = q.SelectMany(p => p.DocumentPostComments).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostComment = q.SelectMany(p => p.DocumentSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.DocumentSubPostComments)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var qPostReaction = q.SelectMany(p => p.DocumentPostReactions).Where(q => !q.IsDelete && q.CreatedOn >= dateToCompare);
        var qSubPostReaction = q.SelectMany(p => p.DocumentSubPosts)
            .Where(q => !q.IsDelete)
            .SelectMany(q => q.DocumentSubPostReactions)
            .Where(x => !x.IsDelete && x.CreatedOn >= dateToCompare);

        var qPostShare = await PostShareGetCount(userId, dateToCompare, (int)PostType.Document);

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

        var totalSharePost = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) >= dateToGetData);

        var totalSharePostBefore = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) < dateToGetData);

        return new InteractionSummary
        {
            CurrentCommentCount = totalComment,
            PreviousCommentCount = totalCommentBefore,
            CurrentReactionCount = totalReaction,
            PreviousReactionCount = totalReactionBefore,
            CurrentShareCount = totalSharePost,
            PreviousShareCount = totalSharePostBefore
        };
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

        var qPostShare = await PostShareGetCount(userId, dateToCompare, (int)PostType.Feed);

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

        var totalSharePost = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) >= dateToGetData);
        var totalSharePostBefore = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) < dateToGetData);

        return await GetInteractions(totalComment + totalReaction + totalSharePost, totalCommentBefore + totalReactionBefore + totalSharePostBefore);
    }

    private async Task<InteractionSummary> GetStoryInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
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

        var qPostShare = await PostShareGetCount(userId, dateToCompare, (int)PostType.Story);

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

        var totalSharePost = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) >= dateToGetData);
        var totalSharePostBefore = qPostShare.Items.Count(p => DateTime.Parse(p.CreatedOn) < dateToGetData);

        return new InteractionSummary
        {
            CurrentCommentCount = totalComment,
            PreviousCommentCount = totalCommentBefore,
            CurrentReactionCount = totalReaction,
            PreviousReactionCount = totalReactionBefore,
            CurrentShareCount = totalSharePost,
            PreviousShareCount = totalSharePostBefore
        };
    }

    private async Task<Interactions> GetComicStoryDocumentInteractionsAsync(Guid? userId, DateTime dateToCompare, DateTime dateToGetData)
    {
        var comic = await GetComicInteractionsAsync(userId, dateToCompare, dateToGetData);
        var story = await GetStoryInteractionsAsync(userId, dateToCompare, dateToGetData);
        var document = await GetDocumentInteractionsAsync(userId, dateToCompare, dateToGetData);

        var totalReactionsLast7Days = comic.CurrentTotal + story.CurrentTotal + document.CurrentTotal;
        var totalReactionsPreviousLast7Days = comic.PreviousTotal + story.PreviousTotal + document.PreviousTotal;

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

    private async Task<TrackingSocialShareGetCountRsp> PostShareGetCount(Guid? userId, DateTime createdOn, int type)
    {
        var res = new TrackingSocialShareGetCountRsp { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new TrackingSocialShareProto.TrackingSocialShareProtoClient(channel);

            var request = new TrackingSocialShareGetCountReq
            {
                UserId = userId + "",
                CreatedOn = createdOn + "",
                Type = type
            };
            var rsp = await client.GetCountAsync(request);

            res.Message = rsp.Message;
            res.Items.AddRange(rsp.Items);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
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
