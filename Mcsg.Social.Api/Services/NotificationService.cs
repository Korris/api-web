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

        var ett = await _context.Available<Notification>().FirstOrDefaultAsync(p => p.Id == request.NotificationId);
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
        var result = await _context.Available<Notification>()
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

    #region -- CheckDataReplyComment --
    private async Task CheckDataReplyComment(IEnumerable<Notification.SearchDto> dtos)
    {
        var list = dtos
            .Where(p => p.EntityType == NotificationEntityType.StoryPostCommentReply
                || p.EntityType == NotificationEntityType.StorySubPostCommentReply
                || p.EntityType == NotificationEntityType.ComicPostCommentReply
                || p.EntityType == NotificationEntityType.ComicSubPostCommentReply
                || p.EntityType == NotificationEntityType.SocialPostCommentReply
                || p.EntityType == NotificationEntityType.SocialSubPostCommentReply)
            .ToList();

        var group = list.GroupBy(p => p.EntityType).ToList();
        foreach (var i in group)
        {
            var replyCommentIds = i.Select(p => p.EntityId).ToList();
            if (replyCommentIds.Count == 0)
            {
                continue;
            }

            var tableName = i.Key switch
            {
                NotificationEntityType.ComicPostCommentReply => $@"comic.""ComicPostComments""",
                NotificationEntityType.ComicSubPostCommentReply => $@"comic.""ComicSubPostComments""",
                NotificationEntityType.SocialPostCommentReply => $@"social.""SocialPostComments""",
                NotificationEntityType.SocialSubPostCommentReply => $@"social.""SocialSubPostComments""",
                NotificationEntityType.StoryPostCommentReply => $@"story.""StoryPostComments""",
                NotificationEntityType.StorySubPostCommentReply => $@"story.""StorySubPostComments""",
                _ => ""
            };

            var replyCommentData = await _notiRepository.Connection.QueryAsync<ReplyCommentData>($@"
                                        SELECT ""Id"" as ReplyCommentId, ""ParentId"" as CommentId
                                        FROM {tableName} 
                                        WHERE ""Id"" = ANY(@ids)", new { ids = replyCommentIds });
            if (!replyCommentData.Any())
            {
                continue;
            }

            var needToMap = list.Where(p => p.EntityType == i.Key);
            foreach (var reply in replyCommentData)
            {
                var data = needToMap.FirstOrDefault(p => p.EntityId == reply.ReplyCommentId);
                if (data == null)
                {
                    continue;
                }

                data.ReplyCommentId = reply.ReplyCommentId;
                data.EntityId = reply.CommentId;
            }
        }
    }

    private async Task CheckDataReplyCommentReaction(IEnumerable<Notification.SearchDto> dtos)
    {
        var list = dtos
            .Where(p => p.EntityType == NotificationEntityType.SocialPostCommentReplyReaction
                || p.EntityType == NotificationEntityType.SocialSubPostCommentReplyReaction
                || p.EntityType == NotificationEntityType.ComicPostCommentReplyReaction
                || p.EntityType == NotificationEntityType.ComicSubPostCommentReplyReaction
                || p.EntityType == NotificationEntityType.StoryPostCommentReplyReaction
                || p.EntityType == NotificationEntityType.StorySubPostCommentReplyReaction)
            .ToList();

        var group = list.GroupBy(p => p.EntityType).ToList();
        foreach (var i in group)
        {
            var replyCommentIds = i.Select(p => p.LocationId).ToList();
            if (replyCommentIds.Count == 0)
            {
                continue;
            }

            var tableName = i.Key switch
            {
                NotificationEntityType.ComicPostCommentReplyReaction => $@"comic.""ComicPostComments""",
                NotificationEntityType.ComicSubPostCommentReplyReaction => $@"comic.""ComicSubPostComments""",
                NotificationEntityType.SocialPostCommentReplyReaction => $@"social.""SocialPostComments""",
                NotificationEntityType.SocialSubPostCommentReplyReaction => $@"social.""SocialSubPostComments""",
                NotificationEntityType.StoryPostCommentReplyReaction => $@"story.""StoryPostComments""",
                NotificationEntityType.StorySubPostCommentReplyReaction => $@"story.""StorySubPostComments""",
                _ => ""
            };

            var query = $@"SELECT pc.""Id"" as ReplyCommentId, pc.""ParentId"" as CommentId , p.""HashId"" as LocationHashId 
                               FROM {tableName} pc
                               LEFT JOIN {tableName.Replace("Comments", "s")} p on pc.""PostId"" = p.""Id""
                               WHERE pc.""Id"" = ANY(@ids)";

            if (i.Key == NotificationEntityType.ComicSubPostCommentReplyReaction || i.Key == NotificationEntityType.StorySubPostCommentReplyReaction)
            {
                var postTable = i.Key switch
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
            if (!replyCommentData.Any())
            {
                continue;
            }

            var needToMap = list.Where(p => p.EntityType == i.Key);
            foreach (var j in needToMap)
            {
                var data = replyCommentData.FirstOrDefault(p => p.ReplyCommentId == j.LocationId);
                if (data == null)
                {
                    continue;
                }

                j.ReplyCommentId = data.ReplyCommentId;
                j.EntityId = data.CommentId;
                j.LocationHashId = data.LocationHashId;
                j.Order = data.Order;
            }
        }
    }
    #endregion

    #region -- CheckDataCommentReaction --
    private async Task CheckDataCommentReaction(IEnumerable<Notification.SearchDto> dtos)
    {
        #region -- Comic --
        var reactionIds = dtos
              .Where(p => p.EntityType == NotificationEntityType.ComicPostCommentReaction
                  || p.EntityType == NotificationEntityType.ComicPostCommentMention)
              .Select(p => p.LocationId);
        await CheckCommentReactionPost<ComicPost, ComicPostComment>(dtos, reactionIds);

        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.ComicSubPostCommentReaction
                || p.EntityType == NotificationEntityType.ComicSubPostCommentMention)
            .Select(p => p.LocationId);
        await CheckCommentReactionSubPost<ComicPost, ComicSubPost, ComicSubPostComment>(dtos, reactionIds);
        #endregion

        #region -- Document --
        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.DocumentPostCommentReaction
                || p.EntityType == NotificationEntityType.DocumentPostCommentMention)
            .Select(p => p.LocationId);
        await CheckCommentReactionPost<DocumentPost, DocumentPostComment>(dtos, reactionIds);

        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.DocumentSubPostCommentReaction
                || p.EntityType == NotificationEntityType.DocumentSubPostCommentMention)
            .Select(p => p.LocationId);
        await CheckCommentReactionSubPost<DocumentPost, DocumentSubPost, DocumentSubPostComment>(dtos, reactionIds);
        #endregion

        #region -- Social --
        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.SocialPostCommentReaction
                || p.EntityType == NotificationEntityType.SocialPostCommentMention)
            .Select(p => p.LocationId)
            .ToList();
        await CheckCommentReactionPost<SocialPost, SocialPostComment>(dtos, reactionIds);

        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.SocialSubPostCommentReaction
                || p.EntityType == NotificationEntityType.SocialSubPostCommentMention)
            .Select(p => p.LocationId);
        await CheckCommentReactionSubPost<SocialPost, SocialSubPost, SocialSubPostComment>(dtos, reactionIds);
        #endregion

        #region -- Story --
        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.StoryPostCommentReaction
                || p.EntityType == NotificationEntityType.StoryPostCommentMention)
            .Select(p => p.LocationId);
        await CheckCommentReactionPost<StoryPost, StoryPostComment>(dtos, reactionIds);

        reactionIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.StorySubPostCommentReaction
                || p.EntityType == NotificationEntityType.StorySubPostCommentMention)
            .Select(p => p.LocationId);
        await CheckCommentReactionSubPost<StoryPost, StorySubPost, StorySubPostComment>(dtos, reactionIds);
        #endregion
    }

    private async Task CheckCommentReactionPost<P, PC>(IEnumerable<Notification.SearchDto> dtos, IEnumerable<Guid?> reactionIds) where P : BasePost where PC : BasePostComment
    {
        if (!reactionIds.Any())
        {
            return;
        }
        var ids = reactionIds.Distinct().ToList();

        var qPost = _context.Set<P>().Where(p => !p.IsDelete);
        var qPostComment = _context.Set<PC>().Where(p => !p.IsDelete);

        var q = from pc in qPostComment
                join p in qPost
                on pc.PostId equals p.Id into pJoined
                from p in pJoined.DefaultIfEmpty()
                where ids.Contains(pc.Id)
                select new
                {
                    CommentId = pc.Id,
                    HashPostId = p.HashId
                };

        var data = await q.ToListAsync();
        foreach (var i in data)
        {
            var dto = dtos.FirstOrDefault(p => p.LocationId == i.CommentId);
            if (dto == null)
            {
                continue;
            }

            dto.LocationHashId = i.HashPostId;
            dto.EntityId = i.CommentId;
        }
    }

    private async Task CheckCommentReactionSubPost<P, SP, PC>(IEnumerable<Notification.SearchDto> dtos, IEnumerable<Guid?> reactionIds) where P : BasePost where SP : BaseSubPost where PC : BasePostComment
    {
        if (!reactionIds.Any())
        {
            return;
        }
        var ids = reactionIds.Distinct().ToList();

        var qPost = _context.Set<P>().Where(p => !p.IsDelete);
        var qSubPost = _context.Set<SP>().Where(p => !p.IsDelete);
        var qPostComment = _context.Set<PC>().Where(p => !p.IsDelete);

        var q = from pc in qPostComment
                join sp in _context.Available<ComicSubPost>()
                on pc.PostId equals sp.Id into spJoined
                from sp in spJoined.DefaultIfEmpty()
                join p in qPost
                on sp.PostId equals p.Id into pJoined
                from p in pJoined.DefaultIfEmpty()
                where ids.Contains(pc.Id)
                select new
                {
                    CommentId = pc.Id,
                    HashPostId = p.HashId,
                    Order = sp == null ? 0f : sp.Order
                };
        var data = await q.ToListAsync();
        foreach (var i in data)
        {
            var dto = dtos.FirstOrDefault(p => p.LocationId == i.CommentId);
            if (dto == null)
            {
                continue;
            }

            dto.LocationHashId = i.HashPostId;
            dto.EntityId = i.CommentId;
            dto.Order = i.Order;
        }
    }
    #endregion

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
        var locationIds = dtos.Where(p => p.LocationId != null).Select(p => p.LocationId).Distinct().ToList();
        if (locationIds.Count == 0)
        {
            return;
        }

        var qPost = _context.Set<P>().Where(p => !p.IsDelete);

        var posts = await qPost.Where(p => locationIds.Contains(p.Id)).Select(p => new { p.Title, p.HashId }).ToListAsync();
        var dic = posts.ToDictionary(p => p.HashId + "", p => p.Title);
        var newDtos = dtos.Where(p => !string.IsNullOrWhiteSpace(p.LocationHashId));
        foreach (var i in newDtos)
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

    #region -- CheckDataCommentOnSubPost --
    private async Task CheckDataCommentOnSubPost(IEnumerable<Notification.SearchDto> dtos)
    {
        // Comic
        var locationIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.ComicSubPostComment
                || p.EntityType == NotificationEntityType.ComicSubPostCommentReply)
            .Select(p => p.LocationId);
        await CheckDataCommentOnSubPost<ComicPost, ComicSubPost>(dtos, locationIds);

        // Document
        locationIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.DocumentSubPostComment
                || p.EntityType == NotificationEntityType.DocumentSubPostCommentReply)
            .Select(p => p.LocationId);
        await CheckDataCommentOnSubPost<DocumentPost, DocumentSubPost>(dtos, locationIds);

        // Social
        locationIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.SocialSubPostComment
                || p.EntityType == NotificationEntityType.SocialSubPostCommentReply)
            .Select(p => p.LocationId);
        await CheckDataCommentOnSubPost<SocialPost, SocialSubPost>(dtos, locationIds);

        // Story
        locationIds = dtos
           .Where(p => p.EntityType == NotificationEntityType.StorySubPostComment
               || p.EntityType == NotificationEntityType.StorySubPostCommentReply)
           .Select(p => p.LocationId);
        await CheckDataCommentOnSubPost<StoryPost, StorySubPost>(dtos, locationIds);
    }

    private async Task CheckDataCommentOnSubPost<P, SP>(IEnumerable<Notification.SearchDto> dtos, IEnumerable<Guid?> locationIds) where P : BasePost where SP : BaseSubPost
    {
        var ids = locationIds.Where(p => p != null).Distinct().ToList();
        if (ids.Count == 0)
        {
            return;
        }

        var qPost = _context.Set<P>().Where(p => !p.IsDelete);
        var qSubPost = _context.Set<SP>().Where(p => !p.IsDelete);

        var list = await (from csp in qSubPost
                          join cp in qPost on csp.PostId equals cp.Id
                          where ids.Contains(csp.Id)
                          select new
                          {
                              csp.Id,
                              csp.Order,
                              cp.HashId
                          }).ToListAsync();
        foreach (var i in list)
        {
            var dto = dtos.FirstOrDefault(p => p.LocationId == i.Id);
            if (dto == null)
            {
                continue;
            }

            dto.Order = i.Order;
            dto.LocationHashId = i.HashId;
        }
    }
    #endregion

    private async Task CheckDataFollowUser(IEnumerable<Notification.SearchDto> dtos)
    {
        var userFollowIds = dtos
            .Where(p => p.EntityType == NotificationEntityType.FollowUser)
            .Select(p => p.EntityId)
            .Distinct().ToList();

        if (userFollowIds.Count > 0)
        {
            return;
        }

        var list = await _context.UserAvailable
            .Where(u => userFollowIds.Contains(u.Id))
            .Select(u => new
            {
                UserId = u.Id,
                u.ProfileName,
                u.UserName,
                u.Avatar
            })
            .ToListAsync();

        foreach (var i in list)
        {
            var dto = dtos.FirstOrDefault(p => p.EntityId == i.UserId);
            if (dto == null)
            {
                continue;
            }

            dto.LocationHashId = i.UserName + "";
        }
    }

    private async Task CheckDataTransaction(IEnumerable<Notification.SearchDto> dtos)
    {
        var l = dtos
            .Where(p => p.EntityType == NotificationEntityType.TransferTransaction
                || p.EntityType == NotificationEntityType.DonateTransaction
                || p.EntityType == NotificationEntityType.DepositTransaction)
            .ToList();
        if (l.Count == 0)
        {
            return;
        }

        var ids = l.Select(p => p.EntityId).Distinct().ToList();
        var data = await GetTransactionFromProto(ids);
        if (data.Count == 0)
        {
            return;
        }

        foreach (var i in l)
        {
            var transactionData = data!.GetValueOrDefault(i.EntityId.ToString());
            if (transactionData == null)
            {
                continue;
            }

            i.Amount = Convert.ToDecimal(transactionData.Amount).ToString("N0");
            i.ReferenceNumber = transactionData.ReferenceNumber;
            i.CurrencyUnit = transactionData.CurrencyUnit;
        }
    }

    private async Task<Dictionary<string, TransactionProtoDto>> GetTransactionFromProto(List<Guid?> transactionIds)
    {
        var res = new Dictionary<string, TransactionProtoDto>();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Wallet.Wallet!);
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
