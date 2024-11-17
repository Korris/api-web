using Dapper;
using Grpc.Net.Client;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services;

using Common.Constants;
using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Models;
using Requests;
using Wallet.Api.Protos;
using static Common.SeedWork.Constants.Error;

public partial class NotificationService : BaseSettingS, INotificationService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="setting">Setting</param>
    /// <param name="unitOfWork"></param>
    public NotificationService(IMcsgContext context, ISetting setting, IUnitOfWork unitOfWork) : base(context, setting)
    {
        _notiRepository = unitOfWork.GetRepository<Notification>();
    }

    public async Task<bool> ReadNotificationAsync(NotificationUpdateR request)
    {
        var userId = request.UserId ?? throw new NotFoundException(nameof(E303), E303);

        var ett = await _context.NotificationAvailable.FirstOrDefaultAsync(p => p.Id == request.NotificationId);
        if (ett == null)
        {
            throw new NotFoundException(ErrorCodes.QueryEmpty, ErrorCodes.QueryEmpty);
        }

        ett.Status = NotificationStatus.Read;
        ett.ModifiedOn = DateTime.UtcNow;
        ett.ModifiedBy = userId;

        return await _context.SaveChangesAsync(default) > 0;
    }

    public async Task<bool> ReadAllNotificationAsync(Guid? userId)
    {
        if (userId == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        // Update Status
        var result = await _context.NotificationAvailable
            .Where(p => p.ReceiverId == userId)
            .ExecuteUpdateAsync(p => p.SetProperty(q => q.Status, NotificationStatus.Read));

        return result > 0;
    }

    public async Task<PagedResponse<Notification.SearchDto>> GetNotificationByReceiverAsync(NotificationR request)
    {
        var userId = request.UserId ?? throw new NotFoundException(nameof(E303), E303);

        var query = GetNotificationByUserQuery;
        query = query.Replace("[UnreadCondition]", "");
        query = query.Replace("[UnreadCountCondition]", "");

        var offset = request.PageSize * (request.PageNumber - 1);
        var param = new
        {
            ReceiverId = userId,
            request.PageSize,
            Offet = offset
        };
        var multi = await _notiRepository.Connection.QueryMultipleAsync(query, param);

        var items = await multi.ReadAsync<Notification.SearchDto>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            await CheckDataReplyComment(items);
            await CheckDataCommentOnSubPost(items);
            await CheckDataFollowPost(items);
            await CheckDataCommentReaction(items);
            await CheckDataReplyCommentReaction(items);
            await CheckDataFollowUser(items);
            await CheckDataTransaction(items);

            var response = new PagedResponse<Notification.SearchDto>(totalItems, request.PageNumber, request.PageSize)
            {
                Items = items
            };

            return response;
        }
        else
        {
            return new PagedResponse<Notification.SearchDto>(0);
        }
    }

    public async Task<PagedResponse<Notification.SearchDto>> GetUnReadNotificationByReceiverAsync(NotificationR request)
    {
        var userId = request.UserId ?? throw new NotFoundException(nameof(E303), E303);

        var query = GetNotificationByUserQuery;
        query = query.Replace("[UnreadCondition]", $@"AND noti.""Status"" = 0");
        query = query.Replace("[UnreadCountCondition]", $@"AND noti.""Status"" = 0");

        var offset = request.PageSize * (request.PageNumber - 1);
        var param = new
        {
            ReceiverId = userId,
            request.PageSize,
            Offet = offset
        };
        var multi = await _notiRepository.Connection.QueryMultipleAsync(query, param);

        var items = await multi.ReadAsync<Notification.SearchDto>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            await CheckDataCommentOnSubPost(items);
            await CheckDataFollowPost(items);
            await CheckDataCommentReaction(items);

            var response = new PagedResponse<Notification.SearchDto>(totalItems, request.PageNumber, request.PageSize)
            {
                Items = items
            };

            return response;
        }
        else
        {
            return new PagedResponse<Notification.SearchDto>(0);
        }
    }

    public async Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/reaction");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
        }
    }

    public async Task<bool> AddMentionNotificationAsync(MentionPostNotificationReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/post-mention");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
        }
    }

    private async Task CheckDataTransaction(IEnumerable<Notification.SearchDto> resDto)
    {
        var listData = resDto.Where(p => p.EntityType == NotificationEntityType.TransferTransaction ||
                                         p.EntityType == NotificationEntityType.DonateTransaction)
            .ToList();

        if (listData.Any())
        {
            var data = await GetTransactionFromProto(listData.Select(p => p.EntityId).Distinct().ToList());
            if (data.Any())
            {
                foreach (var item in listData)
                {
                    var transactionData = data.GetValueOrDefault(item.EntityId.ToString());
                    var amount = transactionData.Amount.ToString("N0");
                    item.Amount = amount;
                    item.ReferenceNumber = transactionData.ReferenceNumber;
                }

            }
        }
    }

    private async Task CheckDataReplyCommentReaction(IEnumerable<Notification.SearchDto> resDto)
    {
        var resReplyCommentReaction = resDto.Where(p => p.EntityType == NotificationEntityType.SocialPostCommentReplyReaction ||
                                                       p.EntityType == NotificationEntityType.SocialSubPostCommentReplyReaction ||
                                                       p.EntityType == NotificationEntityType.ComicPostCommentReplyReaction ||
                                                       p.EntityType == NotificationEntityType.ComicSubPostCommentReplyReaction ||
                                                       p.EntityType == NotificationEntityType.StoryPostCommentReplyReaction ||
                                                       p.EntityType == NotificationEntityType.StorySubPostCommentReplyReaction
                                                       )
                                            .ToList();

        var groupRes = resReplyCommentReaction.GroupBy(p => p.EntityType).ToList();

        foreach (var item in groupRes)
        {
            var replyCommentIds = item.Select(p => p.LocationId).ToList();
            if (replyCommentIds.Count > 0)
            {
                var tableName = item.Key switch
                {
                    NotificationEntityType.StoryPostCommentReplyReaction => $@"story.""StoryPostComments""",
                    NotificationEntityType.ComicPostCommentReplyReaction => $@"Comic.""ComicPostComments""",
                    NotificationEntityType.SocialPostCommentReplyReaction => $@"Social.""SocialPostComments""",
                    NotificationEntityType.SocialSubPostCommentReplyReaction => $@"Social.""SocialSubPostComments""",
                    NotificationEntityType.ComicSubPostCommentReplyReaction => $@"comic.""ComicSubPostComments""",
                    NotificationEntityType.StorySubPostCommentReplyReaction => $@"story.""StorySubPostComments""",
                    _ => ""
                };

                var query = $@"SELECT pc.""Id"" as ReplyCommentId, pc.""ParentId"" as CommentId , p.""HashId"" as LocationHashId 
                               FROM {tableName} pc
                               LEFT JOIN {tableName.Replace("Comments", "s")} p on pc.""PostId"" = p.""Id""
                               WHERE pc.""Id"" = ANY(@ids)";

                if (item.Key == NotificationEntityType.ComicSubPostCommentReplyReaction || item.Key == NotificationEntityType.StorySubPostCommentReplyReaction)
                {
                    var postTable = item.Key switch
                    {
                        NotificationEntityType.ComicSubPostCommentReplyReaction => $@"comic.""ComicPosts""",
                        _ => $@"story.""StoryPosts"""
                    };

                    query = $@"SELECT pc.""Id"" as ReplyCommentId, pc.""ParentId"" as CommentId , p.""HashId"" as LocationHashId , sp.""Order"" 
                               FROM {tableName} pc
                               LEFT JOIN {tableName.Replace("Comments", "s")} sp on pc.""PostId"" = sp.""Id""
                               LEFT JOIN {postTable} as p on p.""Id"" = sp.""PostId""
                               WHERE pc.""Id"" = ANY(@ids)";
                }

                var replyCommentData = await _notiRepository.Connection.QueryAsync<ReplyCommentReactionData>(query, new { ids = replyCommentIds });

                if (replyCommentData.Any())
                {
                    var resNeedToMap = resReplyCommentReaction.Where(p => p.EntityType == item.Key);
                    foreach (var reply in resNeedToMap)
                    {
                        var data = replyCommentData.FirstOrDefault(p => p.ReplyCommentId == reply.LocationId);
                        if (data != null)
                        {
                            reply.ReplyCommentId = data.ReplyCommentId;
                            reply.EntityId = data.CommentId;
                            reply.LocationHashId = data.LocationHashId;
                            reply.Order = data.Order;
                        }
                    }
                }
            }
        }
    }

    private async Task CheckDataCommentReaction(IEnumerable<Notification.SearchDto> resDto)
    {
        var postCommentReactionIds = resDto.Where(p => p.EntityType == NotificationEntityType.SocialPostCommentReaction
                                                     || p.EntityType == NotificationEntityType.SocialPostCommentMention).Select(p => p.LocationId).ToList();
        if (postCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                        SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from social.""SocialPostComments"" pc
                                        LEFT JOIN social.""SocialPosts"" sp on pc.""PostId"" = sp.""Id""
                                        WHERE pc.""Id"" = ANY(@ids)", new { ids = postCommentReactionIds });
            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.EntityId = item.CommentId;
                    }
                }
            }
        }

        var comicPostCommentReactionIds = resDto.Where(p => p.EntityType == NotificationEntityType.ComicPostCommentReaction
                                                          || p.EntityType == NotificationEntityType.ComicPostCommentMention).Select(p => p.LocationId).ToList();
        if (comicPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                        SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from comic.""ComicPostComments"" pc
                                        LEFT JOIN comic.""ComicPosts"" sp on pc.""PostId"" = sp.""Id""
                                        WHERE pc.""Id"" = ANY(@ids)", new { ids = comicPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.EntityId = item.CommentId;
                    }
                }
            }
        }

        var storyPostCommentReactionIds = resDto.Where(p => p.EntityType == NotificationEntityType.StoryPostCommentReaction
                                                         || p.EntityType == NotificationEntityType.StoryPostCommentMention).Select(p => p.LocationId).ToList();
        if (storyPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                  SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from story.""StoryPostComments"" pc
                                  LEFT JOIN story.""StoryPosts"" sp on pc.""PostId"" = sp.""Id""
                                  WHERE pc.""Id"" = ANY(@ids)", new { ids = storyPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.EntityId = item.CommentId;
                    }
                }
            }
        }

        var comicSubPostCommentReactionIds = resDto.Where(p => p.EntityType == NotificationEntityType.ComicSubPostCommentReaction
                                                            || p.EntityType == NotificationEntityType.ComicSubPostCommentMention).Select(p => p.LocationId).ToList();
        if (comicSubPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                        SELECT pc.""Id"" as CommentId,p.""HashId"" as HashPostId, sp.""Order"" from comic.""ComicSubPostComments"" pc
                                        LEFT JOIN comic.""ComicSubPosts"" sp on pc.""PostId"" = sp.""Id""
                                        LEFT JOIN comic.""ComicPosts"" p on sp.""PostId"" = p.""Id""
                                        WHERE pc.""Id"" = ANY(@ids)", new { ids = comicSubPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.EntityId = item.CommentId;
                        response.Order = item.Order;
                    }
                }
            }
        }

        var storySubPostCommentReactionIds = resDto.Where(p => p.EntityType == NotificationEntityType.StorySubPostCommentReaction
                                                       || p.EntityType == NotificationEntityType.StorySubPostCommentMention).Select(p => p.LocationId).ToList();
        if (storySubPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                 SELECT pc.""Id"" as CommentId,p.""HashId"" as HashPostId, sp.""Order"" from story.""StorySubPostComments"" pc
                                 LEFT JOIN story.""StorySubPosts"" sp on pc.""PostId"" = sp.""Id""
                                 LEFT JOIN story.""StoryPosts"" p on sp.""PostId"" = p.""Id""
                                 WHERE pc.""Id"" = ANY(@ids)", new { ids = storySubPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.EntityId = item.CommentId;
                        response.Order = item.Order;
                    }
                }
            }
        }

        var subPostCommentReactionIds = resDto.Where(p => p.EntityType == NotificationEntityType.SocialSubPostCommentReaction).Select(p => p.LocationId).ToList();
        if (subPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                 SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from social.""SocialSubPostComments"" pc
                                 LEFT JOIN social.""SocialSubPosts"" sp on pc.""PostId"" = sp.""Id""
                                 LEFT JOIN social.""SocialPosts"" p on sp.""PostId"" = p.""Id""
                                 WHERE pc.""Id"" = ANY(@ids)", new { ids = subPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId &&
                                                         p.EntityType == NotificationEntityType.SocialSubPostCommentReaction);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.EntityId = item.CommentId;
                    }
                }
            }
        }
    }

    #region -- CheckDataFollowPost --
    private async Task CheckDataFollowPost(IEnumerable<Notification.SearchDto> dtos)
    {
        var list = dtos.Where(p => p.EntityType == NotificationEntityType.ComicPostFollow).ToList();
        await CheckDataFollowPost<ComicPost>(dtos, NotificationContent.FollowComic);

        list = dtos.Where(p => p.EntityType == NotificationEntityType.DocumentPostFollow).ToList();
        await CheckDataFollowPost<DocumentPost>(dtos, NotificationContent.FollowDocument);

        list = dtos.Where(p => p.EntityType == NotificationEntityType.StoryPostFollow).ToList();
        await CheckDataFollowPost<StoryPost>(dtos, NotificationContent.FollowStory);
    }

    private async Task CheckDataFollowPost<P>(IEnumerable<Notification.SearchDto> dtos, string content) where P : BasePost
    {
        var locationIds = dtos.Select(p => p.LocationId).Distinct().ToList();
        if (locationIds.Count == 0)
        {
            return;
        }

        var qPost = _context.Set<P>().Where(p => !p.IsDelete);

        var posts = await qPost.Where(p => locationIds.Contains(p.Id)).Select(p => new { p.Title, p.HashId }).ToListAsync();
        var dic = posts.ToDictionary(p => p.HashId + "", p => p.Title);
        foreach (var i in dtos)
        {
            var title = dic.GetValueOrDefault(i.LocationHashId);
            if (title == null)
            {
                continue;
            }

            i.FollowPostMessage = string.Format(content, i.ActorName, title);
        }
    }
    #endregion

    private async Task CheckDataReplyComment(IEnumerable<Notification.SearchDto> resDto)
    {
        var res = resDto.Where(p => p.EntityType == NotificationEntityType.StoryPostCommentReply ||
                                    p.EntityType == NotificationEntityType.StorySubPostCommentReply ||
                                    p.EntityType == NotificationEntityType.ComicPostCommentReply ||
                                    p.EntityType == NotificationEntityType.ComicSubPostCommentReply ||
                                    p.EntityType == NotificationEntityType.SocialPostCommentReply ||
                                    p.EntityType == NotificationEntityType.SocialSubPostCommentReply
        ).ToList();

        var groupRes = res.GroupBy(p => p.EntityType).ToList();

        foreach (var item in groupRes)
        {
            var replyCommentIds = item.Select(p => p.EntityId).ToList();
            if (replyCommentIds.Count > 0)
            {
                var tableName = item.Key switch
                {
                    NotificationEntityType.StoryPostCommentReply => $@"story.""StoryPostComments""",
                    NotificationEntityType.StorySubPostCommentReply => $@"story.""StorySubPostComments""",
                    NotificationEntityType.ComicPostCommentReply => $@"Comic.""ComicPostComments""",
                    NotificationEntityType.ComicSubPostCommentReply => $@"Comic.""ComicSubPostComments""",
                    NotificationEntityType.SocialPostCommentReply => $@"Social.""SocialPostComments""",
                    NotificationEntityType.SocialSubPostCommentReply => $@"Social.""SocialSubPostComments""",
                    _ => ""
                };
                var replyCommentData = await _notiRepository.Connection.QueryAsync<ReplyCommentData>($@"
                                        SELECT ""Id"" as ReplyCommentId, ""ParentId"" as CommentId
                                        FROM {tableName} 
                                        WHERE ""Id"" = ANY(@ids)", new { ids = replyCommentIds });

                if (replyCommentData.Any())
                {
                    var resNeedToMap = res.Where(p => p.EntityType == item.Key);
                    foreach (var reply in replyCommentData)
                    {
                        var data = resNeedToMap.FirstOrDefault(p => p.EntityId == reply.ReplyCommentId);
                        if (data != null)
                        {
                            data.ReplyCommentId = reply.ReplyCommentId;
                            data.EntityId = reply.CommentId;
                        }
                    }
                }
            }
        }
    }

    private async Task CheckDataCommentOnSubPost(IEnumerable<Notification.SearchDto> resDto)
    {
        var subComicIds = resDto.Where(p => p.EntityType == NotificationEntityType.ComicSubPostComment || p.EntityType == NotificationEntityType.ComicSubPostCommentReply).Select(p => p.LocationId).ToList();
        if (subComicIds.Count > 0)
        {
            var subComics = await _notiRepository.Connection.QueryAsync<SubPostData>($@"
                                        SELECT csp.""Id"",csp.""Order"",cp.""HashId""  from comic.""ComicSubPosts"" csp
                                        JOIN comic.""ComicPosts"" cp on csp.""PostId"" = cp.""Id""
                                        WHERE csp.""Id"" = ANY(@ids)", new { ids = subComicIds });
            if (subComics.Count() > 0)
            {
                foreach (var item in subComics)
                {
                    var comicResponse = resDto.FirstOrDefault(p => p.LocationId == item.Id);
                    if (comicResponse != null)
                    {
                        comicResponse.Order = item.Order;
                        comicResponse.LocationHashId = item.HashId;
                    }
                }
            }
        }

        var subStoryIds = resDto.Where(p => p.EntityType == NotificationEntityType.StorySubPostComment || p.EntityType == NotificationEntityType.StorySubPostCommentReply).Select(p => p.LocationId).ToList();
        if (subStoryIds.Count > 0)
        {
            var subStories = await _notiRepository.Connection.QueryAsync<SubPostData>($@"
                                        SELECT csp.""Id"",csp.""Order"",cp.""HashId""  from story.""StorySubPosts"" csp
                                        JOIN story.""StoryPosts"" cp on csp.""PostId"" = cp.""Id""
                                        WHERE csp.""Id"" = ANY(@SubPostIds)", new { SubPostIds = subStoryIds });
            if (subStories.Count() > 0)
            {
                foreach (var item in subStories)
                {
                    var storyResponse = resDto.FirstOrDefault(p => p.LocationId == item.Id);
                    if (storyResponse != null)
                    {
                        storyResponse.Order = item.Order;
                        storyResponse.LocationHashId = item.HashId;
                    }
                }
            }
        }
    }

    private async Task CheckDataFollowUser(IEnumerable<Notification.SearchDto> resDto)
    {
        var userFollowIds = resDto
            .Where(p => p.EntityType == NotificationEntityType.FollowUser)
            .Select(p => p.EntityId)
            .ToList();

        if (userFollowIds.Count > 0)
        {
            var userData = await _notiRepository.Connection.QueryAsync<UserFollowedResponse>($@"
                SELECT u.""Id"" as UserId,  
                       u.""ProfileName"", 
                       u.""UserName"", 
                       u.""Avatar""
                FROM identity.""Users"" u
                WHERE u.""Id"" = ANY(@ids)", new { ids = userFollowIds });

            if (userData.Count() > 0)
            {
                foreach (var item in userData)
                {
                    var response = resDto.FirstOrDefault(p => p.EntityId == item.UserId);
                    if (response != null)
                    {
                        response.LocationHashId = item.UserName + "";
                    }
                }
            }
        }
    }

    private async Task<Dictionary<string, TransactionProtoDto>> GetTransactionFromProto(List<Guid?> transactionIds)
    {
        var res = new Dictionary<string, TransactionProtoDto>();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Web.Wallet!);

            var client = new UserWalletProto.UserWalletProtoClient(channel);
            var request = new TransactionGetReq
            {
                TransactionId = string.Join(';', transactionIds.Where(id => id != null).Select(id => id.ToString()))
            };
            var rsp = await client.GetTransactionInfoAsync(request);
            return rsp.Transactions.ToDictionary(p => p.TransactionId, p => p);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }

        return res;
    }

    #endregion

    #region -- Fields --

    private readonly IRepository<Notification> _notiRepository;

    #endregion
}
