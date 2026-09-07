using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Constants;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Notifications;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Models.RealTime;
using Common.SeedWork.Extensions;
using Mcsg.Api.Areas.Realtime.Constants;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Hubs;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;
using static Common.Core.Constants.Message;
using static Common.Core.Constants.Setting;
using NotificationType = Common.Core.Constants.Setting.NotificationType;

public class NotificationService : BaseS, INotificationService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context">DB context</param>
    /// <param name="hc">Notification hub</param>
    /// <param name="nc">Notification client</param>
    public NotificationService(IMcsgContext context, IHubContext<NotificationHub> hc, INotificationClient nc,
        IWebHostEnvironment environment) : base(context)
    {
        _hc = hc;
        _nc = nc;
        _basePath = Path.Combine(environment.ContentRootPath, "Translation");
    }

    public async Task<NotificationResponse> AddTransactionNotification(TransactionNotificationReq req)
    {
        var notificationEntityType = req.TransactionType switch
        {
            TransactionType.Transfer => NotificationEntityType.TransferTransaction,
            TransactionType.Donate => NotificationEntityType.DonateTransaction,
            TransactionType.Deposit => NotificationEntityType.DepositTransaction,
            TransactionType.BuyPremium => req.IsRenewPremium
                ? NotificationEntityType.BuyRenewPremiumTransaction
                : req.IsUpgradePremium
                    ? NotificationEntityType.BuyUpgradePremiumTransaction
                    : NotificationEntityType.BuyPremiumTransaction,
            _ => NotificationEntityType.TransferTransaction
        };

        var noti = await AddNotificationAsync(
            actorId: req.AuthorId,
            receiverId: req.ReceiverId,
            action: NotificationAction.Transaction,
            entityType: notificationEntityType,
            entityId: req.Id);

        var user = await _context.UserAvailable
            .Where(p => p.Id == req.AuthorId)
            .Select(p => new { p.ProfileName, p.Avatar, p.PremiumDate })
            .FirstOrDefaultAsync();

        var response = new NotificationResponse
        {
            Id = noti.Id,
            Amount = req.Amount.ToString(),
            Status = noti.Status,
            EntityId = req.Id,
            ReferenceNumber = req.ReferenceNumber,
            ActorId = req.AuthorId,
            ActorName = user?.ProfileName ?? "",
            Message = GetMessageTransaction(notificationEntityType),
            CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow,
            NotificationType = GetTransactionType(notificationEntityType),
            UserAvatar = user?.Avatar,
            CurrencyUnit = req.CurrencyUnit,
            ExpiredDateOriginal = user?.PremiumDate,
            ExpiredDate = user?.PremiumDate?.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
        };

        var responseNotify = JsonConvert.SerializeObject(response);
        await _hc.Clients.Group(req.ReceiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, responseNotify);
        await SendFireBaseNotification(new List<Guid> { req.ReceiverId }, response, "Giao dịch thành công");

        if (req.TransactionType == TransactionType.Deposit)
        {
            await _hc.Clients.Group(req.ReceiverId.ToString()).SendAsync(RealTimeTopic.ReceiveDepositSucces, responseNotify);
        }

        return response;
    }

    public async Task<NotificationResponse> AddCommentNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();

        if (!string.IsNullOrWhiteSpace(comment.PostHashId))
        {
            var receiverId = comment.PostCreatedBy;

            // Dont notify when comment on their feed
            if (receiverId != comment.AuthorId)
            {
                var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: receiverId
                                        , action: comment.IsReply ? NotificationAction.Reply : NotificationAction.Comment
                                        , entityType: comment.EntityType
                                        , entityId: comment.Id
                                        , locationId: comment.PostId
                                        , locationHashId: comment.PostHashId);

                response.Id = noti.Id;
                response.Status = noti.Status;
                response.LocationId = comment.PostId;
                response.LocationHashId = comment.PostHashId;
                response.EntityId = comment.Id;
                response.CommentId = comment.Id;
                response.Message = GetMessage(comment);
                response.TargetType = GetTargetType(comment);
                response.ActorId = comment.AuthorId;
                response.ActorName = comment.AuthorName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = NotificationType.Comment;
                response.UserAvatar = comment.UserAvatar;
                response.Order = comment.Order ?? 0;
                response.TotalCreatedBy = await _context.Available<NotificationObject>().Where(p => p.LocationId == response.LocationId && p.EntityType == comment.EntityType).Select(p => p.ActorId).Distinct().CountAsync();
                await _hc.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
                await SendFireBaseNotification(new List<Guid> { receiverId }, response, "FocFoc");
            }
        }

        return response;
    }

    public async Task<NotificationResponse> AddReplyNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();

        var id = comment.QuoteId ?? comment.ReplyToCommentId;
        var postId = comment.PostId;
        IQueryable<Guid> qAuthorId = default!;
        IQueryable<string?> qHashId = default!;

        var targetType = GetTargetType(comment);
        switch (targetType)
        {
            case NotificationTargetType.Comic:
                qAuthorId = _context.Available<ComicPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<ComicPost>().Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubComic:
                qAuthorId = _context.Available<ComicSubPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<ComicPost>().Where(p => p.ComicSubPosts.Any(p => p.Id == postId)).Select(p => p.HashId);
                break;

            case NotificationTargetType.Document:
                qAuthorId = _context.Available<DocumentPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<DocumentPost>().Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubDocument:
                qAuthorId = _context.Available<DocumentSubPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<DocumentPost>().Where(p => p.DocumentSubPosts.Any(p => p.Id == postId)).Select(p => p.HashId);
                break;

            case NotificationTargetType.Social:
                qAuthorId = _context.Available<SocialPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<SocialPost>().Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubSocial:
                qAuthorId = _context.Available<SocialSubPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<SocialSubPost>().Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.Story:
                qAuthorId = _context.Available<StoryPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<StoryPost>().Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubStory:
                qAuthorId = _context.Available<StorySubPostComment>().Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.Available<StoryPost>().Where(p => p.StorySubPosts.Any(p => p.Id == postId)).Select(p => p.HashId);
                break;
        }

        var receiverId = await qAuthorId.FirstOrDefaultAsync();

        // Dont notify when comment on their feed
        if (receiverId != Guid.Empty && receiverId != comment.AuthorId)
        {
            var hashId = await qHashId.FirstOrDefaultAsync();

            var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: receiverId
                                        , action: comment.IsReply ? NotificationAction.Reply : NotificationAction.Comment
                                        , entityType: comment.EntityType
                                        , entityId: comment.Id
                                        , locationId: comment.PostId
                                        , locationHashId: hashId + "");

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = comment.PostId;
            response.LocationHashId = hashId + "";
            response.EntityId = comment.Id;
            response.Message = nameof(NotificationContent.ReplyOnComment);
            response.TargetType = targetType;
            response.ActorId = comment.AuthorId;
            response.ActorName = comment.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.Reply;
            response.UserAvatar = comment.UserAvatar;
            response.Order = comment.Order ?? 0;
            response.ReplyCommentId = comment.Id;
            response.CommentId = comment.ReplyToCommentId;
            response.TotalCreatedBy = await _context.Available<NotificationObject>().Where(p => p.LocationId == response.LocationId && p.EntityType == comment.EntityType).Select(p => p.ActorId).Distinct().CountAsync();
            await _hc.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            await SendFireBaseNotification(new List<Guid> { receiverId }, response, "FocFoc");
        }

        return response;
    }

    public async Task<NotificationResponse> AddVideoNotification(VideoNotificationR video)
    {
        var receiverId = video.AuthorId;
        var postHashId = video.PostHashId + "";

        var response = new NotificationResponse()
        {
            Status = NotificationStatus.UnRead,
            LocationId = video.PostId,
            LocationHashId = postHashId,
            EntityHashId = video.HashId + "",
            Message = GetVideoMessage(video.Action),
            TargetType = !string.IsNullOrWhiteSpace(video.TargetType) ? video.TargetType : NotificationTargetType.None,
            ActorId = video.AuthorId,
            ActorName = video.AuthorName + "",
            UserAvatar = video.UserAvatar,
            NotificationType = NotificationType.Video + video.Action.ToString(),
            CreatedOn = DateTime.UtcNow
        };

        // When Processing no need to save Notification to database
        if (video.Action != NotificationAction.Processing)
        {
            var noti = await AddNotificationAsync(
                                  actorId: video.AuthorId
                                  , receiverId: receiverId
                                  , action: video.Action
                                  , entityType: NotificationEntityType.Video
                                  , entityId: video.Id
                                  , locationId: video.PostId
                                  , locationHashId: postHashId);
            response.Id = noti.Id;
        }

        await _hc.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

        return response;
    }

    public async Task<NotificationResponse> AddReactionNotification(ReactionNotificationReq reaction)
    {
        var response = new NotificationResponse();

        var noti = new NotificationDto();
        var receiverId = Guid.Empty;
        var postId = Guid.Empty;
        var locationId = Guid.Empty;
        var postHashId = "";
        var locationHashId = "";
        var targetId = reaction.TargetId;
        bool isCommentReaction = reaction.EntityType != NotificationEntityType.SocialPostReaction
            && reaction.EntityType != NotificationEntityType.StoryPostReaction
            && reaction.EntityType != NotificationEntityType.ComicPostReaction
            && reaction.EntityType != NotificationEntityType.DocumentPostReaction
            && reaction.EntityType != NotificationEntityType.SocialSubPostReaction;

        // Reaction for comment => Find Id of Post
        if (isCommentReaction)
        {
            var qComment = _context.Available<SocialPostComment>().Where(p => p.Id == targetId)
                .Select(p => new
                {
                    p.Id,
                    p.PostId,
                    p.AuthorId,
                    p.ParentId,
                });

            var entityType = reaction.EntityType;
            switch (entityType)
            {
                case NotificationEntityType.SocialPostCommentReaction or NotificationEntityType.SocialPostCommentReplyReaction:
                    qComment = _context.Available<SocialPostComment>().Where(p => p.Id == targetId)
                        .Select(p => new
                        {
                            p.Id,
                            p.PostId,
                            p.AuthorId,
                            p.ParentId,
                        });
                    break;

                case NotificationEntityType.ComicPostCommentReaction or NotificationEntityType.ComicPostCommentReplyReaction:
                    qComment = _context.Available<ComicPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.DocumentPostCommentReaction or NotificationEntityType.DocumentPostCommentReplyReaction:
                    qComment = _context.Available<DocumentPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.StoryPostCommentReaction or NotificationEntityType.StoryPostCommentReplyReaction:
                    qComment = _context.Available<StoryPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.SocialSubPostCommentReaction or NotificationEntityType.SocialSubPostCommentReplyReaction:
                    qComment = _context.Available<SocialSubPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.ComicSubPostCommentReaction or NotificationEntityType.SocialPostCommentReplyReaction:
                    qComment = _context.Available<ComicSubPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.DocumentSubPostCommentReaction or NotificationEntityType.SocialPostCommentReplyReaction:
                    qComment = _context.Available<DocumentSubPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.StorySubPostCommentReaction or NotificationEntityType.StorySubPostCommentReplyReaction:
                    qComment = _context.Available<StorySubPostComment>().Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                default:
                    break;
            }

            var comment = await qComment.FirstOrDefaultAsync();
            if (comment == null)
            {
                return response;
            }

            targetId = comment.PostId;
            receiverId = comment.AuthorId;
            locationId = reaction.TargetId;
            response.CommentId = reaction.IsReplyReaction ? comment.ParentId : reaction.TargetId;

            if (reaction.IsReplyReaction)
            {
                response.ReplyCommentId = reaction.TargetId;
            }

            response.Message = nameof(NotificationContent.ReactOnComment);
        }

        var q = _context.Available<SocialPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0f, PostId = _uidEmpty });
        switch (reaction.EntityType)
        {
            case NotificationEntityType.SocialPostReaction:
            case NotificationEntityType.SocialPostCommentReaction:
            case NotificationEntityType.SocialPostCommentReplyReaction:
                break;

            case NotificationEntityType.ComicPostReaction:
            case NotificationEntityType.ComicPostCommentReaction:
            case NotificationEntityType.ComicPostCommentReplyReaction:
                q = _context.Available<ComicPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0f, PostId = _uidEmpty });
                break;

            case NotificationEntityType.DocumentPostReaction:
            case NotificationEntityType.DocumentPostCommentReaction:
            case NotificationEntityType.DocumentPostCommentReplyReaction:
                q = _context.Available<DocumentPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0f, PostId = _uidEmpty });
                break;

            case NotificationEntityType.StoryPostReaction:
            case NotificationEntityType.StoryPostCommentReaction:
            case NotificationEntityType.StoryPostCommentReplyReaction:
                q = _context.Available<StoryPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0f, PostId = _uidEmpty });
                break;

            case NotificationEntityType.SocialSubPostReaction:
            case NotificationEntityType.SocialSubPostCommentReaction:
            case NotificationEntityType.SocialSubPostCommentReplyReaction:
                q = _context.Available<SocialSubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = (float)p.Order, p.PostId });
                break;

            case NotificationEntityType.ComicSubPostCommentReaction:
            case NotificationEntityType.ComicSubPostCommentReplyReaction:
                q = _context.Available<ComicSubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, p.Order, p.PostId });
                break;

            case NotificationEntityType.DocumentSubPostCommentReaction:
            case NotificationEntityType.DocumentSubPostCommentReplyReaction:
                q = _context.Available<DocumentSubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, p.Order, p.PostId });
                break;

            case NotificationEntityType.StorySubPostCommentReaction:
            case NotificationEntityType.StorySubPostCommentReplyReaction:
                q = _context.Available<StorySubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, p.Order, p.PostId });
                break;

            default:
                break;
        }

        var ett = await q.FirstOrDefaultAsync();
        if (ett != null)
        {
            postId = ett.Id;
            postHashId = ett.HashId;

            // Only reaction by subpost comment need order to go to subpost Comic/Document/Story
            if (reaction.EntityType == NotificationEntityType.ComicSubPostCommentReaction || reaction.EntityType == NotificationEntityType.DocumentSubPostCommentReaction || reaction.EntityType == NotificationEntityType.StorySubPostCommentReaction ||
                reaction.EntityType == NotificationEntityType.ComicSubPostCommentReplyReaction || reaction.EntityType == NotificationEntityType.DocumentSubPostCommentReplyReaction || reaction.EntityType == NotificationEntityType.StorySubPostCommentReplyReaction)
            {
                response.Order = ett.Order;
                Guid postIdOfSubPost = ett.PostId;
                IQueryable<string?> qHashId = default!;

                switch (reaction.EntityType)
                {
                    case NotificationEntityType.ComicSubPostCommentReaction:
                        qHashId = _context.Available<ComicPost>().Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.DocumentSubPostCommentReaction:
                        qHashId = _context.Available<DocumentPost>().Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.StorySubPostCommentReaction:
                        qHashId = _context.Available<StoryPost>().Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.ComicSubPostCommentReplyReaction:
                        qHashId = _context.Available<ComicPost>().Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.DocumentSubPostCommentReplyReaction:
                        qHashId = _context.Available<DocumentPost>().Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.StorySubPostCommentReplyReaction:
                        qHashId = _context.Available<StoryPost>().Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    default:
                        break;
                }

                postHashId = await qHashId.FirstOrDefaultAsync() + "";
            }

            if (!isCommentReaction)
            {
                var notiContent = reaction.EntityType switch
                {
                    NotificationEntityType.ComicPostReaction => nameof(NotificationContent.ReactOnComic),
                    NotificationEntityType.DocumentPostReaction => nameof(NotificationContent.ReactOnDocument),
                    NotificationEntityType.StoryPostReaction => nameof(NotificationContent.ReactOnStory),
                    _ => nameof(NotificationContent.ReactOnFeed),
                };

                receiverId = ett.UserId;
                locationId = ett.Id;
                locationHashId = ett.HashId;
                response.Message = notiContent;
            }

            response.TargetType = reaction.EntityType switch
            {
                NotificationEntityType.SocialPostReaction or NotificationEntityType.SocialPostCommentReaction or NotificationEntityType.SocialPostCommentReplyReaction => NotificationTargetType.Social,
                NotificationEntityType.StoryPostReaction or NotificationEntityType.StoryPostCommentReaction or NotificationEntityType.StoryPostCommentReplyReaction => NotificationTargetType.Story,
                NotificationEntityType.ComicPostReaction or NotificationEntityType.ComicPostCommentReaction or NotificationEntityType.ComicPostCommentReplyReaction => NotificationTargetType.Comic,
                NotificationEntityType.DocumentPostReaction or NotificationEntityType.DocumentPostCommentReaction or NotificationEntityType.DocumentPostCommentReplyReaction => NotificationTargetType.Document,
                NotificationEntityType.SocialSubPostReaction or NotificationEntityType.SocialSubPostCommentReaction or NotificationEntityType.SocialSubPostCommentReplyReaction => NotificationTargetType.SubSocial,
                NotificationEntityType.ComicSubPostCommentReaction or NotificationEntityType.ComicSubPostCommentReplyReaction => NotificationTargetType.SubComic,
                NotificationEntityType.DocumentSubPostCommentReaction or NotificationEntityType.DocumentSubPostCommentReplyReaction => NotificationTargetType.SubDocument,
                NotificationEntityType.StorySubPostCommentReaction or NotificationEntityType.StorySubPostCommentReplyReaction => NotificationTargetType.SubStory,
                _ => string.Empty
            };

            var notificationObject = await _context.NotificationObjects.Where(p => p.EntityType == reaction.EntityType
                                                                                  && p.ActorId == reaction.AuthorId
                                                                                  && p.EntityId == reaction.Id).FirstOrDefaultAsync();
            if (notificationObject != null)
            {
                notificationObject.CreatedOn = DateTime.UtcNow;
                var notification = await _context.Notifications.FirstOrDefaultAsync(p => p.NotificationObjectId == notificationObject.Id);

                await _context.Notifications
                    .Where(p => p.NotificationObjectId == notificationObject.Id)
                    .ExecuteUpdateAsync(x => x
                        .SetProperty(p => p.Status, NotificationStatus.UnRead)
                        .SetProperty(p => p.CreatedOn, DateTime.UtcNow)
                        .SetProperty(p => p.IsDelete, false));

                response.Id = notification == null ? _uidEmpty : notification.Id;
                response.Status = NotificationStatus.UnRead;
                response.LocationId = postId;
                response.LocationHashId = postHashId + "";
                response.EntityId = reaction.Id;
                response.ActorId = reaction.AuthorId;
                response.ActorName = reaction.AuthorName;
                response.CreatedOn = DateTime.UtcNow;
                response.NotificationType = NotificationType.Reaction;
                response.UserAvatar = reaction.UserAvatar;
                response.ReactionType = reaction.ReactionType;
            }
            else if (receiverId != Guid.Empty && receiverId != reaction.AuthorId)
            {
                noti = await AddNotificationAsync(
                                  actorId: reaction.AuthorId
                                  , receiverId: receiverId
                                  , action: NotificationAction.Reaction
                                  , entityType: reaction.EntityType
                                  , entityId: reaction.Id
                                  , locationId: locationId
                                  , locationHashId: locationHashId + "");

                response.Id = noti.Id;
                response.Status = noti.Status;
                response.LocationId = postId;
                response.LocationHashId = postHashId + "";
                response.EntityId = reaction.Id;
                response.ActorId = reaction.AuthorId;
                response.ActorName = reaction.AuthorName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = NotificationType.Reaction;
                response.UserAvatar = reaction.UserAvatar;
                response.ReactionType = reaction.ReactionType;
            }

            response.TotalCreatedBy = await _context.Available<NotificationObject>().Where(p => p.LocationId == response.LocationId && p.EntityType == reaction.EntityType).Select(p => p.ActorId).Distinct().CountAsync();

            await _hc.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            await SendFireBaseNotification(new List<Guid> { receiverId }, response, "FocFoc");
        }

        return response;
    }

    public async Task AddPostMentionNotification(MentionPostNotificationReq request)
    {
        var response = new NotificationResponse();
        var receiverId = Guid.Empty;
        var postId = Guid.Empty;
        var locationId = Guid.Empty;
        var postHashId = "";
        var locationHashId = "";
        var targetId = request.TargetId;
        bool isMentionComment = request.EntityType != NotificationEntityType.SocialPostMention;

        // Reaction for comment => Find Id of Post
        if (isMentionComment)
        {
            var qPostId = request.EntityType switch
            {
                NotificationEntityType.ComicPostCommentMention => _context.Available<ComicPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),
                NotificationEntityType.DocumentPostCommentMention => _context.Available<DocumentPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),
                NotificationEntityType.SocialPostCommentMention => _context.Available<SocialPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),
                NotificationEntityType.StoryPostCommentMention => _context.Available<StoryPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),

                NotificationEntityType.ComicSubPostCommentMention => _context.Available<ComicSubPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),
                NotificationEntityType.DocumentSubPostCommentMention => _context.Available<DocumentSubPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),
                NotificationEntityType.SocialSubPostCommentMention => _context.Available<SocialSubPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),
                NotificationEntityType.StorySubPostCommentMention => _context.Available<StorySubPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId),

                _ => _context.Available<SocialPostComment>().Where(p => p.Id == request.TargetId).Select(p => p.PostId)
            };

            targetId = await qPostId.FirstOrDefaultAsync();
            locationId = request.TargetId;
            response.CommentId = request.TargetId;
        }

        var qPost = request.EntityType switch
        {
            NotificationEntityType.ComicPostCommentMention => _context.Available<ComicPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, Order = 0f, PostId = _uidEmpty }),
            NotificationEntityType.DocumentPostCommentMention => _context.Available<DocumentPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, Order = 0f, PostId = _uidEmpty }),
            NotificationEntityType.SocialPostMention => _context.Available<SocialPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, Order = 0f, PostId = _uidEmpty }),
            NotificationEntityType.StoryPostCommentMention => _context.Available<StoryPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, Order = 0f, PostId = _uidEmpty }),

            NotificationEntityType.ComicSubPostCommentMention => _context.Available<ComicSubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.Order, p.PostId }),
            NotificationEntityType.DocumentSubPostCommentMention => _context.Available<DocumentSubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.Order, p.PostId }),
            NotificationEntityType.SocialSubPostCommentMention => _context.Available<SocialSubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, Order = (float)p.Order, p.PostId }),
            NotificationEntityType.StorySubPostCommentMention => _context.Available<StorySubPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.Order, p.PostId }),

            _ => _context.Available<SocialPost>().Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, Order = 0f, PostId = _uidEmpty }),
        };

        var post = await qPost.FirstOrDefaultAsync();
        if (post != null)
        {
            postId = post.Id;
            postHashId = post.HashId;

            // Only reaction by subpost comment need order to go to subpost Comic/Document/Story
            if (request.EntityType == NotificationEntityType.ComicSubPostCommentMention || request.EntityType == NotificationEntityType.DocumentSubPostCommentMention || request.EntityType == NotificationEntityType.StorySubPostCommentMention)
            {
                response.Order = post.Order;
                postHashId = request.EntityType == NotificationEntityType.DocumentSubPostCommentMention
                    ? await _context.Available<DocumentPost>().Where(p => p.Id == post.PostId).Select(p => p.HashId).FirstOrDefaultAsync()
                    : await _context.Available<StoryPost>().Where(p => p.Id == post.PostId).Select(p => p.HashId).FirstOrDefaultAsync();
            }

            if (!isMentionComment)
            {
                locationId = post.Id;
                locationHashId = post.HashId;
            }

            response.TargetType = request.EntityType switch
            {
                NotificationEntityType.ComicPostCommentMention => NotificationTargetType.Comic,
                NotificationEntityType.DocumentPostCommentMention => NotificationTargetType.Document,
                NotificationEntityType.SocialPostMention or NotificationEntityType.SocialPostCommentMention => NotificationTargetType.Social,
                NotificationEntityType.StoryPostCommentMention => NotificationTargetType.Story,

                NotificationEntityType.ComicSubPostCommentMention => NotificationTargetType.SubComic,
                NotificationEntityType.DocumentSubPostCommentMention => NotificationTargetType.SubDocument,
                NotificationEntityType.SocialSubPostMention or NotificationEntityType.SocialSubPostCommentReaction => NotificationTargetType.SubSocial,
                NotificationEntityType.StorySubPostCommentMention => NotificationTargetType.SubStory,

                _ => NotificationTargetType.Social
            };

            foreach (var item in request.ReceiversId)
            {

                var noti = await AddNotificationAsync(
                                    actorId: request.UserId
                                  , receiverId: item
                                  , action: NotificationAction.Mention
                                  , entityType: request.EntityType
                                  , entityId: item
                                  , locationId: locationId
                                  , locationHashId: locationHashId + "");

                response.Id = noti.Id;
                response.Status = noti.Status;
                response.LocationId = postId;
                response.LocationHashId = postHashId + "";
                response.EntityId = item;
                response.ActorId = request.UserId;
                response.UserAvatar = request.UserAvatar;
                response.ActorName = request.UserProfileName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = NotificationType.Mention;
                response.Message = (isMentionComment ? nameof(NotificationContent.MentionOnComment) : nameof(NotificationContent.MentionOnPost));

                await _hc.Clients.Group(item.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }

            await SendFireBaseNotification(request.ReceiversId, response, "FocFoc");
        }
    }

    public async Task<NotificationResponse> AddMentionNotification(MentionNotificationReq mention)
    {
        // TODO
        var response = new NotificationResponse();

        var receiverId = mention.EntityId;

        // Dont notify when comment on their feed
        if (receiverId != mention.AuthorId)
        {
            var entityType = NotificationEntityType.SocialPostCommentMention;
            var targetType = NotificationTargetType.CommentOnFeed;
            if (mention.LocationType == MentionLocationType.SubPostComment || mention.LocationType == MentionLocationType.SubPostCommentReply)
            {
                entityType = NotificationEntityType.SocialSubPostCommentMention;
                targetType = NotificationTargetType.CommentOnSubFeed;
            }

            var noti = await AddNotificationAsync(
                                    actorId: mention.AuthorId
                                    , receiverId: receiverId
                                    , action: NotificationAction.Mention
                                    , entityType: entityType
                                    , entityId: mention.EntityId
                                    , locationId: mention.LocationId
                                    , locationHashId: mention.PostHashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = mention.LocationId;
            response.LocationHashId = mention.PostHashId;
            response.EntityId = mention.EntityId;
            response.Message = nameof(NotificationContent.MentionOnComment);
            response.TargetType = targetType;
            response.ActorId = mention.AuthorId;
            response.ActorName = mention.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.Mention;
            response.UserAvatar = mention.UserAvatar;

            await _hc.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            await SendFireBaseNotification(new List<Guid> { receiverId }, response, "FocFoc");
        }

        return response;
    }

    /// <summary>
    /// AddNotificationAsync
    /// </summary>
    /// <param name="actorId"></param>
    /// <param name="receiverId"></param>
    /// <param name="action"></param>
    /// <param name="entityType"></param>
    /// <param name="status"></param>
    /// <param name="entityId"></param>
    /// <param name="locationId"></param>
    /// <param name="locationHashId"></param>
    /// <param name="entityhashId"></param>
    /// <returns></returns>
    public async Task<NotificationDto> AddNotificationAsync(Guid actorId, Guid receiverId, NotificationAction action, NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead, Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        var notiObj = new NotificationObject
        {
            EntityType = entityType,
            Action = action,
            EntityId = entityId,
            EntityHashId = entityhashId,
            ActorId = actorId,
            LocationId = locationId,
            LocationHashId = locationHashId,
            CreatedBy = actorId,
        };
        await _context.NotificationObjects.AddAsync(notiObj);

        var noti = new Notification()
        {
            NotificationObjectId = notiObj.Id,
            ReceiverId = receiverId,
            Status = status
        };
        await _context.Notifications.AddAsync(noti);
        await _context.SaveChangesAsync(default);

        var res = new NotificationDto
        {
            Id = noti.Id,
            Status = noti.Status,
            NotificationObjectId = notiObj.Id,
            ReceiverId = receiverId,
            ActorId = actorId,
            CreatedOn = noti.CreatedOn
        };

        return res;
    }

    public async Task<List<NotificationDto>> AddNotificationsAsync(Guid actorId, List<Guid> receiverIds, NotificationAction action, NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead, Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        var res = new List<NotificationDto>();

        var notiObj = new NotificationObject
        {
            EntityType = entityType,
            Action = action,
            EntityId = entityId,
            EntityHashId = entityhashId,
            ActorId = actorId,
            LocationId = locationId,
            LocationHashId = locationHashId,
            CreatedBy = actorId,
        };
        await _context.NotificationObjects.AddAsync(notiObj);

        foreach (var receiverId in receiverIds)
        {
            var noti = new Notification
            {
                NotificationObjectId = notiObj.Id,
                ReceiverId = receiverId,
                Status = status
            };
            await _context.Notifications.AddAsync(noti);

            var dto = new NotificationDto
            {
                Id = noti.Id,
                Status = noti.Status,
                NotificationObjectId = notiObj.Id,
                ReceiverId = receiverId,
                ActorId = actorId,
                CreatedOn = noti.CreatedOn
            };

            res.Add(dto);
        }

        await _context.SaveChangesAsync(default);

        return res;
    }

    public async Task AddTransactionUpdate(RealTimeTransactionUpdateReq req)
    {
        if (req != null && req.UserId != Guid.Empty && !string.IsNullOrEmpty(req.TransactionId))
        {
            await _hc.Clients.Group(req.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveTransactionUpdate, JsonConvert.SerializeObject(req));
        }
    }

    public async Task AddCommonNotification(CommonNotificationReq req)
    {
        if (req != null && !string.IsNullOrEmpty(req.TopicName) && !string.IsNullOrEmpty(req.Message))
        {
            if (!string.IsNullOrEmpty(req.UserId))
            {
                await _hc.Clients.Group(req.UserId).SendAsync(req.TopicName, req.Message);
            }
            else
            {
                await _hc.Clients.All.SendAsync(req.TopicName, req.Message);
            }
        }
    }

    public async Task<NotificationResponse> AddFollowPostNotification(FollowPostNotificationReq request)
    {
        var response = new NotificationResponse();

        var notiTargetType = request.NotificationEntityType switch
        {
            NotificationEntityType.ComicPostFollow => NotificationTargetType.Comic,
            NotificationEntityType.DocumentPostFollow => NotificationTargetType.Document,
            _ => NotificationTargetType.Story,
        };

        var notiContent = request.NotificationEntityType switch
        {
            NotificationEntityType.ComicPostFollow => nameof(NotificationContent.FollowComic),
            NotificationEntityType.DocumentPostFollow => nameof(NotificationContent.FollowDocument),
            _ => nameof(NotificationContent.FollowStory),
        };

        var notificationObject = await _context.NotificationObjects
            .Where(p => p.EntityType == request.NotificationEntityType &&
                        p.ActorId == request.ActorId &&
                        p.LocationId == request.PostId)
            .FirstOrDefaultAsync();

        if (notificationObject != null)
        {
            notificationObject.CreatedOn = DateTime.UtcNow;
            var notification = await _context.Notifications.FirstOrDefaultAsync(p => p.NotificationObjectId == notificationObject.Id);

            await _context.Notifications
                .Where(p => p.NotificationObjectId == notificationObject.Id)
                .ExecuteUpdateAsync(x => x
                    .SetProperty(p => p.Status, NotificationStatus.UnRead)
                    .SetProperty(p => p.CreatedOn, DateTime.UtcNow));


            response.Id = notification!.Id;
            response.Status = NotificationStatus.UnRead;
            response.LocationId = request.PostId;
            response.LocationHashId = request.PostHashId;
            response.Message = notiContent;
            response.TargetType = notiTargetType;
            response.ActorId = request.ActorId;
            response.ActorName = request.ActorName;
            response.CreatedOn = notificationObject?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.FollowPost;
            response.UserAvatar = request.UserAvatar;
            response.PostName = request.PostName;
        }
        else
        {
            var noti = await AddNotificationAsync(
                                    actorId: request.ActorId
                                    , receiverId: request.ReceiverId
                                    , action: NotificationAction.FollowPost
                                    , entityType: request.NotificationEntityType
                                    , entityId: request.ReceiverId
                                    , locationId: request.PostId
                                    , locationHashId: request.PostHashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = request.PostId;
            response.LocationHashId = request.PostHashId;
            response.Message = notiContent;
            response.TargetType = notiTargetType;
            response.ActorId = request.ActorId;
            response.ActorName = request.ActorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.FollowPost;
            response.UserAvatar = request.UserAvatar;
            response.PostName = request.PostName;
        }

        await _hc.Clients.Group(request.ReceiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        await SendFireBaseNotification(new List<Guid> { request.ReceiverId }, response, "FocFoc");

        return response;
    }

    public async Task RemindExpiredSubscriptionNotification(RemindExpiredSubscriptionR request)
    {
        var response = new NotificationResponse();

        foreach (var i in request.RemindDatas)
        {
            var noti = await AddNotificationAsync(
                          actorId: i.UserId
                        , receiverId: i.UserId
                        , action: NotificationAction.Remind
                        , entityType: NotificationEntityType.RemindExpiredSubscription);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.Message = NotificationType.RemindExpiredSubscription;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.RemindExpiredSubscription;
            response.ExpiredDateOriginal = i.ExpiredDate;
            response.ExpiredDate = i.ExpiredDate.ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
            await _hc.Clients.Group(i.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            await SendFireBaseNotification(new List<Guid> { i.UserId }, response, "FocFoc");
        }
    }

    public async Task ExpiredSubscriptionNotification(ExpiredSubscriptionR request)
    {
        var response = new NotificationResponse();

        foreach (var userId in request.UserIds)
        {
            var noti = await AddNotificationAsync(
                          actorId: userId
                        , receiverId: userId
                        , action: NotificationAction.Remind
                        , entityType: NotificationEntityType.ExpiredSubscription);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.Message = NotificationType.ExpiredSubscription;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.ExpiredSubscription;
            await _hc.Clients.Group(userId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }
        await SendFireBaseNotification(request.UserIds, response, "FocFoc");
    }

    public async Task<NotificationResponse> AddRejecton(NotificationAddRejectionR request)
    {
        var response = new NotificationResponse();

        var report = await _context.SocialReports
            .Where(p => p.Id == request.EntityId && p.ModifiedBy != null)
            .Select(p => new
            {
                p.EntityType,
                p.EntityId,
                ModifiedBy = p.ModifiedBy!.Value
            })
            .FirstOrDefaultAsync();

        if (report == null)
        {
            return response;
        }

        var details = await _context.Available<SocialReportDetail>()
            .Where(p => p.ReportId == request.EntityId && p.Status != ReportDetailStatus.SentNotification)
            .Select(p => new
            {
                p.UserId,
                p.Id
            })
            .ToListAsync();

        var type = request.EntityType.ToEnum(EntityType.CommentPost);

        var notiEntityType = (type == EntityType.CommentPost || type == EntityType.CommentSubPost)
            ? NotificationEntityType.RejectCommentReport
            : NotificationEntityType.RejectPostReport;

        var notiAction = (type == EntityType.CommentPost || type == EntityType.CommentSubPost)
            ? NotificationAction.RejectCommentReport
            : NotificationAction.RejectPostReport;

        var notiTargetType = (type == EntityType.CommentPost || type == EntityType.CommentSubPost)
            ? NotificationTargetType.RejectCommentReport
            : NotificationTargetType.RejectPostReport;

        var message = (type == EntityType.CommentPost || type == EntityType.CommentSubPost)
            ? nameof(S306)
            : nameof(S307);

        foreach (var i in details)
        {
            var noti = await AddNotificationAsync(
                        actorId: report.ModifiedBy
                        , receiverId: i.UserId
                        , action: notiAction
                        , entityType: notiEntityType
                        , entityId: i.Id
                        , locationId: report.EntityId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.EntityId = request.EntityId;
            response.Message = message;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.TargetType = notiTargetType;
            response.NotificationType = NotificationType.RejectReport;

            await _hc.Clients.Group(i.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

        await _context.SocialReportDetails
            .Where(p => p.ReportId == request.EntityId && p.Status != ReportDetailStatus.SentNotification)
            .ExecuteUpdateAsync(p => p.SetProperty(q => q.Status, ReportDetailStatus.SentNotification));

        return response;
    }

    public async Task<NotificationResponse> FollowNotification(UserFollowResp followResp)
    {
        var response = new NotificationResponse();

        if (followResp.CreatedByUserId != Guid.Empty)
        {
            var receiverId = followResp.FollowedId;

            if (receiverId != followResp.CreatedByUserId)
            {
                var notiObj = await (from a in _context.Available<NotificationObject>()
                                     join b in _context.Notifications on a.Id equals b.NotificationObjectId
                                     where a.ActorId == followResp.CreatedByUserId
                                           && b.ReceiverId == followResp.FollowedId
                                           && a.Action == NotificationAction.FollowUser
                                     select new
                                     {
                                         a.Id,
                                         a.CreatedOn
                                     }).FirstOrDefaultAsync();

                if (notiObj != null)
                {
                    await _context.Available<Notification>()
                        .Where(p => p.NotificationObjectId == notiObj.Id)
                        .ExecuteUpdateAsync(x => x
                            .SetProperty(p => p.Status, p => NotificationStatus.UnRead)
                            .SetProperty(p => p.CreatedOn, p => DateTime.UtcNow)
                            .SetProperty(p => p.ModifiedOn, p => DateTime.UtcNow));
                    await _context.Available<NotificationObject>()
                        .Where(p => p.Id == notiObj.Id)
                        .ExecuteUpdateAsync(x => x
                            .SetProperty(p => p.CreatedOn, p => DateTime.UtcNow)
                            .SetProperty(p => p.ModifiedOn, p => DateTime.UtcNow));
                }
                else
                {
                    var noti = await AddNotificationAsync(
                                        actorId: followResp.CreatedByUserId
                                        , receiverId: receiverId
                                        , action: NotificationAction.FollowUser
                                        , entityType: NotificationEntityType.FollowUser
                                        , entityId: followResp.CreatedByUserId
                                        , locationHashId: followResp.CreatedByUserName);

                    response.Id = noti.Id;
                    response.Status = noti.Status;
                    response.LocationId = null;
                    response.LocationHashId = followResp.CreatedByUserName;
                    response.EntityId = null;
                    response.Message = nameof(NotificationContent.FollowUser);
                    response.TargetType = NotificationTargetType.FollowUser;
                    response.ActorId = followResp.CreatedByUserId;
                    response.ActorName = followResp.CreatedByUserName;
                    response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                    response.NotificationType = NotificationType.FollowUser;
                    response.UserAvatar = followResp.CreatedByUserAvata;

                    await _hc.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
                    await SendFireBaseNotification(new List<Guid> { receiverId }, response, "FocFoc");

                    return response;
                }
            }
        }

        return response;
    }

    public async Task<NotificationResponse> AddDeletion(NotificationAddDeletionR request)
    {
        var response = new NotificationResponse();

        var q = _context.ComicPosts
            .Where(p => p.Id == request.EntityId)
            .Select(p => new
            {
                p.ModifiedBy,
                CreatedBy = p.UserId
            });

        var type = request.NotificationType.ToEnum(AddDeletionType.ComicPost);
        switch (type)
        {
            case AddDeletionType.ComicSubPost:
                q = _context.ComicSubPosts
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.UserId
                   });
                break;

            case AddDeletionType.ComicPostComment:
                q = _context.ComicPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.ComicSubPostComment:
                q = _context.ComicSubPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.DocumentPost:
                q = _context.DocumentPosts
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.UserId
                   });
                break;

            case AddDeletionType.DocumentSubPost:
                q = _context.DocumentSubPosts
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.UserId
                   });
                break;

            case AddDeletionType.DocumentPostComment:
                q = _context.DocumentPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.DocumentSubPostComment:
                q = _context.DocumentSubPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.SocialPost:
                q = _context.SocialPosts
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.UserId
                   });
                break;

            case AddDeletionType.SocialPostComment:
                q = _context.SocialPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.SocialSubPostComment:
                q = _context.SocialSubPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.StoryPost:
                q = _context.StoryPosts
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.UserId
                   });
                break;

            case AddDeletionType.StorySubPost:
                q = _context.StorySubPosts
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.UserId
                   });
                break;

            case AddDeletionType.StoryPostComment:
                q = _context.StoryPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;

            case AddDeletionType.StorySubPostComment:
                q = _context.StorySubPostComments
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       CreatedBy = p.CreatedBy!.Value
                   });
                break;
            default:
                break;
        }

        var ett = await q.FirstOrDefaultAsync();
        if (ett == null)
        {
            return response;
        }

        var action = type switch
        {
            AddDeletionType.ComicSubPost or AddDeletionType.DocumentSubPost or AddDeletionType.StorySubPost => NotificationAction.DeleteSubPost,
            AddDeletionType.ComicPost or AddDeletionType.DocumentPost or AddDeletionType.StoryPost or AddDeletionType.SocialPost => NotificationAction.DeletePost,
            _ => NotificationAction.DeleteComment
        };

        var targetType = type switch
        {
            AddDeletionType.ComicPost => NotificationTargetType.Comic,
            AddDeletionType.ComicSubPost => NotificationTargetType.SubComic,
            AddDeletionType.ComicPostComment => NotificationTargetType.CommentOnComic,
            AddDeletionType.ComicSubPostComment => NotificationTargetType.CommentOnSubComic,

            AddDeletionType.StoryPost => NotificationTargetType.Story,
            AddDeletionType.StorySubPost => NotificationTargetType.SubStory,
            AddDeletionType.StoryPostComment => NotificationTargetType.CommentOnStory,
            AddDeletionType.StorySubPostComment => NotificationTargetType.CommentOnSubStory,

            AddDeletionType.DocumentPost => NotificationTargetType.Document,
            AddDeletionType.DocumentSubPost => NotificationTargetType.SubDocument,
            AddDeletionType.DocumentPostComment => NotificationTargetType.CommentOnDocument,
            AddDeletionType.DocumentSubPostComment => NotificationTargetType.CommentOnSubDocument,

            AddDeletionType.SocialPost => NotificationTargetType.Social,
            AddDeletionType.SocialPostComment => NotificationTargetType.CommentOnFeed,
            AddDeletionType.SocialSubPostComment => NotificationTargetType.CommentOnSubFeed,

            _ => NotificationTargetType.Social
        };

        var message = type switch
        {
            AddDeletionType.ComicPost or AddDeletionType.StoryPost => nameof(S300),
            AddDeletionType.ComicSubPost or AddDeletionType.StorySubPost => nameof(S301),
            AddDeletionType.DocumentPost or AddDeletionType.StoryPost => nameof(S300),
            AddDeletionType.DocumentSubPost or AddDeletionType.StorySubPost => nameof(S301),
            AddDeletionType.SocialPost => nameof(S302),
            _ => nameof(S308)
        };

        var notiType = type switch
        {
            AddDeletionType.ComicPost or AddDeletionType.StoryPost => NotificationType.DeletePost,
            AddDeletionType.ComicSubPost or AddDeletionType.StorySubPost => NotificationType.DeleteSubPost,
            AddDeletionType.DocumentPost or AddDeletionType.StoryPost => NotificationType.DeletePost,
            AddDeletionType.DocumentSubPost or AddDeletionType.StorySubPost => NotificationType.DeleteSubPost,
            AddDeletionType.SocialPost => NotificationType.DeleteSocial,
            _ => NotificationType.DeleteComment
        };

        var entityType = type switch
        {
            AddDeletionType.ComicPost => NotificationEntityType.ComicPostDelete,
            AddDeletionType.ComicPostComment => NotificationEntityType.ComicPostCommentDelete,
            AddDeletionType.ComicSubPost => NotificationEntityType.ComicSubPostDelete,
            AddDeletionType.ComicSubPostComment => NotificationEntityType.ComicSubPostCommentDelete,

            AddDeletionType.DocumentPost => NotificationEntityType.DocumentPostDelete,
            AddDeletionType.DocumentPostComment => NotificationEntityType.DocumentPostCommentDelete,
            AddDeletionType.DocumentSubPost => NotificationEntityType.DocumentSubPostDelete,
            AddDeletionType.DocumentSubPostComment => NotificationEntityType.DocumentSubPostCommentDelete,

            AddDeletionType.SocialPostComment => NotificationEntityType.SocialPostCommentDelete,
            AddDeletionType.SocialSubPostComment => NotificationEntityType.SocialSubPostCommentDelete,

            AddDeletionType.StoryPost => NotificationEntityType.StoryPostDelete,
            AddDeletionType.StoryPostComment => NotificationEntityType.StoryPostCommentDelete,
            AddDeletionType.StorySubPost => NotificationEntityType.StorySubPostDelete,
            AddDeletionType.StorySubPostComment => NotificationEntityType.StorySubPostCommentDelete,

            _ => NotificationEntityType.SocialPostDelete
        };

        var noti = await AddNotificationAsync(
                                actorId: ett.ModifiedBy!.Value
                                , receiverId: ett.CreatedBy
                                , action: action
                                , entityType: entityType
                                , entityId: request.EntityId
                                , locationId: request.EntityId);

        response.Id = noti.Id;
        response.EntityId = request.EntityId;
        response.Status = noti.Status;
        response.LocationId = request.EntityId;
        response.Message = message;
        response.TargetType = targetType;
        response.ActorId = ett.ModifiedBy.Value;
        response.CreatedOn = noti.CreatedOn;
        response.NotificationType = notiType;

        await _hc.Clients.Group(ett.CreatedBy.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

        return response;
    }

    public async Task<NotificationResponse> AddLock(NotificationAddLockR request)
    {
        var response = new NotificationResponse();

        var q = from report in _context.Available<ComicReport>()
                join reportDetail in _context.Available<ComicReportDetail>() on report.Id equals reportDetail.ReportId
                join post in _context.Available<ComicPost>() on report.EntityId equals post.Id
                where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                orderby reportDetail.CreatedOn descending
                select new
                {
                    report.Id,
                    report.EntityId,
                    ReportUserId = reportDetail.UserId,
                    post.UserId
                };

        var action = NotificationAction.LockPost;
        var targetType = NotificationTargetType.Comic;
        var message = nameof(S303);
        var notiType = NotificationType.LockPost;
        var entityType = NotificationEntityType.ComicPostLock;

        var type = request.NotificationType.ToEnum(AddLockType.ComicPost);
        switch (type)
        {
            case AddLockType.ComicSubPost:
                q = from report in _context.Available<ComicReport>()
                    join reportDetail in _context.Available<ComicReportDetail>() on report.Id equals reportDetail.ReportId
                    join post in _context.Available<ComicSubPost>() on report.EntityId equals post.Id
                    where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                    orderby reportDetail.CreatedOn descending
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        ReportUserId = reportDetail.UserId,
                        post.UserId
                    };

                action = NotificationAction.LockSubPost;
                targetType = NotificationTargetType.SubComic;
                message = nameof(S304);
                notiType = NotificationType.LockSubPost;
                entityType = NotificationEntityType.ComicSubPostLock;
                break;

            case AddLockType.DocumentPost:
                q = from report in _context.Available<DocumentReport>()
                    join reportDetail in _context.Available<DocumentReportDetail>() on report.Id equals reportDetail.ReportId
                    join post in _context.Available<DocumentPost>() on report.EntityId equals post.Id
                    where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                    orderby reportDetail.CreatedOn descending
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        ReportUserId = reportDetail.UserId,
                        post.UserId
                    };

                action = NotificationAction.LockPost;
                targetType = NotificationTargetType.Document;
                message = nameof(S304);
                notiType = NotificationType.LockSubPost;
                entityType = NotificationEntityType.DocumentPostLock;
                break;

            case AddLockType.DocumentSubPost:
                q = from report in _context.Available<DocumentReport>()
                    join reportDetail in _context.Available<DocumentReportDetail>() on report.Id equals reportDetail.ReportId
                    join post in _context.Available<DocumentSubPost>() on report.EntityId equals post.Id
                    where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                    orderby reportDetail.CreatedOn descending
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        ReportUserId = reportDetail.UserId,
                        post.UserId
                    };

                action = NotificationAction.LockSubPost;
                targetType = NotificationTargetType.SubDocument;
                message = nameof(S304);
                notiType = NotificationType.LockSubPost;
                entityType = NotificationEntityType.DocumentSubPostLock;
                break;

            case AddLockType.SocialPost:
                q = from report in _context.Available<SocialReport>()
                    join reportDetail in _context.Available<SocialReportDetail>() on report.Id equals reportDetail.ReportId
                    join post in _context.Available<SocialPost>() on report.EntityId equals post.Id
                    where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                    orderby reportDetail.CreatedOn descending
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        ReportUserId = reportDetail.UserId,
                        post.UserId
                    };

                action = NotificationAction.LockPost;
                targetType = NotificationTargetType.Social;
                message = nameof(S305);
                notiType = NotificationType.LockSocial;
                entityType = NotificationEntityType.SocialPostLock;
                break;

            case AddLockType.StoryPost:
                q = from report in _context.Available<StoryReport>()
                    join reportDetail in _context.Available<StoryReportDetail>() on report.Id equals reportDetail.ReportId
                    join post in _context.Available<StoryPost>() on report.EntityId equals post.Id
                    where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                    orderby reportDetail.CreatedOn descending
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        ReportUserId = reportDetail.UserId,
                        post.UserId
                    };

                action = NotificationAction.LockPost;
                targetType = NotificationTargetType.Story;
                message = nameof(S303);
                notiType = NotificationType.LockPost;
                entityType = NotificationEntityType.StoryPostLock;
                break;

            case AddLockType.StorySubPost:
                q = from report in _context.Available<StoryReport>()
                    join reportDetail in _context.Available<StoryReportDetail>() on report.Id equals reportDetail.ReportId
                    join post in _context.Available<StorySubPost>() on report.EntityId equals post.Id
                    where report.Id == request.EntityId && reportDetail.TagData != null && reportDetail.TagData.Contains(TagData.Admin)
                    orderby reportDetail.CreatedOn descending
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        ReportUserId = reportDetail.UserId,
                        post.UserId
                    };

                action = NotificationAction.LockSubPost;
                targetType = NotificationTargetType.SubStory;
                message = nameof(S304);
                notiType = NotificationType.LockSubPost;
                entityType = NotificationEntityType.StorySubPostLock;
                break;

            default:
                break;
        }

        var ett = await q.FirstOrDefaultAsync();
        if (ett == null)
        {
            return response;
        }

        var noti = await AddNotificationAsync(
                                actorId: ett.ReportUserId
                                , receiverId: ett.UserId
                                , action: action
                                , entityType: entityType
                                , entityId: ett.Id
                                , locationId: ett.EntityId);

        response.Id = noti.Id;
        response.EntityId = request.EntityId;
        response.Status = noti.Status;
        response.LocationId = ett.EntityId;
        response.Message = message;
        response.TargetType = targetType;
        response.ActorId = ett.ReportUserId;
        response.CreatedOn = noti.CreatedOn;
        response.NotificationType = notiType;
        response.EntityType = entityType;

        await _hc.Clients.Group(ett.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

        return response;
    }

    private string GetTargetType(CommentNotificationReq comment)
    {
        switch (comment.PostType)
        {
            case PostType.Comic:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Comic : NotificationTargetType.SubComic;

            case PostType.Document:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Document : NotificationTargetType.SubDocument;

            case PostType.Story:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Story : NotificationTargetType.SubStory;

            default:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Social : NotificationTargetType.SubSocial;
        }
    }

    private string GetMessageTransaction(NotificationEntityType notificationEntityType)
    {
        return notificationEntityType switch
        {
            NotificationEntityType.TransferTransaction => nameof(NotificationContent.TransferTransaction),
            NotificationEntityType.DonateTransaction => nameof(NotificationContent.DonateTransaction),
            NotificationEntityType.DepositTransaction => nameof(NotificationContent.DepositTransaction),
            NotificationEntityType.BuyPremiumTransaction => nameof(NotificationContent.BuyPremiumTransaction),
            NotificationEntityType.BuyUpgradePremiumTransaction => nameof(NotificationContent.BuyUpgradePremiumTransaction),
            NotificationEntityType.BuyRenewPremiumTransaction => nameof(NotificationContent.BuyRenewPremiumTransaction),
            _ => string.Empty
        };
    }

    private string GetTransactionType(NotificationEntityType notificationEntityType)
    {
        return notificationEntityType switch
        {
            NotificationEntityType.TransferTransaction => NotificationType.TransferTransaction,
            NotificationEntityType.DonateTransaction => NotificationType.DonateTransaction,
            NotificationEntityType.DepositTransaction => NotificationType.DepositTransaction,
            NotificationEntityType.BuyPremiumTransaction => NotificationType.BuyPremiumTransaction,
            NotificationEntityType.BuyUpgradePremiumTransaction => NotificationType.BuyUpgradePremiumTransaction,
            NotificationEntityType.BuyRenewPremiumTransaction => NotificationType.BuyRenewPremiumTransaction,
            _ => string.Empty
        };
    }

    private string GetMessage(CommentNotificationReq comment)
    {
        switch (comment.PostType)
        {
            case PostType.Comic:
                return nameof(NotificationContent.CommentOnComic);

            case PostType.Document:
                return nameof(NotificationContent.CommentOnDocument);

            case PostType.Story:
                return nameof(NotificationContent.CommentOnStory);

            default:
                return nameof(NotificationContent.CommentOnFeed);
        }
    }

    private string GetVideoMessage(NotificationAction action)
    {
        return action switch
        {
            NotificationAction.Processing => nameof(NotificationContent.VideoUploadProcessing),
            NotificationAction.Completed => nameof(NotificationContent.VideoUploadCompleted),
            NotificationAction.Failed => nameof(NotificationContent.VideoUploadFailed),
            _ => throw new NotSupportedException($"Unsupported video action: {action}"),
        };
    }

    private async Task SendFireBaseNotification(List<Guid> receiverIds, NotificationResponse response, string subject)
    {
        var deviceInfos = await _context.Available<Device>(false)
             .Where(p => receiverIds.Contains(p.UserId))
             .Join(_context.Users,
                device => device.UserId,
                user => user.Id,
                (device, user) => new
                {
                    device.Token,
                    device.UserId,
                    Language = user.Language ?? LanguageCode.en,
                })
             .ToListAsync();

        if (deviceInfos.Count <= 0) return;

        var data = response.GetType()
                            .GetProperties()
                            .ToDictionary(prop => prop.Name.ToCamelCase(),
                                          prop => (prop.GetValue(response)?.ToString() ?? ""));

        _nc.SetStrategy(new NotificationFirebase());

        await Parallel.ForEachAsync(deviceInfos, async (deviceInfo, cancellationToken) =>
        {
            var localizedBody = GetLocalizedMessage(response.Message, deviceInfo.Language);
            var body = GetMessageNotification(response.Message, localizedBody, response, deviceInfo.Language);
            await SendFireBase(deviceInfo.Token, subject, body, data);
        });
    }

    private async Task SendFireBase(string token, string subject, string body, Dictionary<string, string> data)
    {
        await _nc.Handle(new NotificationInfoDto(token)
        {
            To = token,
            Subject = subject,
            Body = body,
            Data = data
        });
    }

    private string GetLocalizedMessage(string messageKey, string language)
    {
        string filePath = Path.Combine(_basePath, $"{language}.json");
        string json = File.ReadAllText(filePath);
        JObject jObj = JObject.Parse(json);

        var messageProperty = jObj.Descendants()
                                  .OfType<JProperty>()
                                  .FirstOrDefault(p => p.Name == messageKey);

        if (messageProperty != null)
        {
            return messageProperty.Value.ToString();
        }

        return messageKey;
    }

    private string GetMessageNotification(string messageKey, string message, NotificationResponse response, string languageCode)
    {
        var parameters = new Dictionary<string, string>();
        switch (messageKey)
        {
            case "CommentOnComic":
            case "CommentOnDocument":
            case "CommentOnFeed":
            case "CommentOnStory":
            case "ReactOnComic":
            case "ReactOnDocument":
            case "ReactOnFeed":
            case "ReactOnStory":
            case "ReplyOnComment":
            case "MentionOnComment":
            case "MentionOnPost":
            case "MentionOnReply":
            case "ReactOnComment":
            case "ReactOnReply":
            case "VideoUploadProcessing":
            case "VideoUploadCompleted":
            case "VideoUploadFailed":
            case "FollowUser":
            case "DonateTransaction":
                parameters = new Dictionary<string, string>()
                {
                    ["actorName"] = response.ActorName
                };
                break;

            case "FollowComic":
            case "FollowDocument":
            case "FollowStory":
                parameters = new Dictionary<string, string>()
                {
                    ["actorName"] = response.ActorName,
                    ["postName"] = response.PostName
                };
                break;

            case "TransferTransaction":
                parameters = new Dictionary<string, string>()
                {
                    ["amount"] = response.Amount.FormatCurrency(languageCode),
                    ["currencyUnit"] = response.CurrencyUnit,
                    ["actorName"] = response.ActorName,
                };
                break;

            case "DepositTransaction":
                parameters = new Dictionary<string, string>()
                {
                    ["amount"] = response.Amount.FormatCurrency(languageCode),
                    ["currencyUnit"] = response.CurrencyUnit
                };
                break;

            case "RemindExpiredSubscription":
            case NotificationType.BuyUpgradePremiumTransaction:
            case NotificationType.BuyRenewPremiumTransaction:
                parameters = new Dictionary<string, string>()
                {
                    ["expiredDate"] = response.ExpiredDateOriginal?.ToString("HH:mm, dd.MM.yyyy")
                };
                break;

            default:
                break;
        }

        foreach (var param in parameters)
        {
            message = message.Replace($"{{{{{param.Key}}}}}", param.Value);
        }

        return message;
    }

    public async Task AddSubPostNotification(NotificationAddSubPostR request)
    {
        var notificationEntityType = request.PostType switch
        {
            PostType.Comic => NotificationEntityType.ComicSubPostAdd,
            PostType.Document => NotificationEntityType.DocumentSubPostAdd,
            _ => NotificationEntityType.StorySubPostAdd
        };

        var targetType = request.PostType switch
        {
            PostType.Comic => NotificationTargetType.ComicSubPostAdd,
            PostType.Document => NotificationTargetType.DocumentSubPostAdd,
            _ => NotificationTargetType.StorySubPostAdd,
        };

        var notis = await AddNotificationsAsync(
                      actorId: request.AuthorId
                    , receiverIds: request.FollowerUserIds
                    , action: NotificationAction.AddSubPost
                    , entityType: notificationEntityType
                    , entityId: request.SubPostId
                    , locationId: request.PostId
                    , locationHashId: request.PostHashId);

        foreach (var noti in notis)
        {
            var response = new NotificationResponse
            {
                Id = noti.Id,
                Status = noti.Status,
                EntityId = request.SubPostId,
                Message = nameof(NotificationContent.AddSubPost),
                CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow,
                NotificationType = NotificationType.AddSubPost,
                PostName = request.PostName,
                EntityType = notificationEntityType,
                TargetType = targetType,
                LocationHashId = request.PostHashId,
                Order = request.Order,
                PostThumbnailUrl = request.PostThumbnailUrl
            };
            await _hc.Clients.Group(noti.ReceiverId.ToString())
                .SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            await SendFireBaseNotification(new List<Guid> { noti.ReceiverId }, response, "FocFoc");
        }
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// HubContext
    /// </summary>
    private readonly IHubContext<NotificationHub> _hc;

    /// <summary>
    /// Notification client
    /// </summary>
    private readonly INotificationClient _nc;

    /// <summary>
    /// UID empty
    /// </summary>
    private readonly Guid _uidEmpty = Guid.Empty;

    /// <summary>
    /// BasePath
    /// </summary>
    private readonly string _basePath;

    #endregion
}
