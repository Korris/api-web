using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Services;

using Common.Constants;
using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.Models.RealTime;
using Common.SeedWork.Extensions;
using Constants;
using Dtos;
using Hubs;
using Interfaces;
using Requests;
using Wallet.Domain.Enums;
using static Common.Core.Constants.Message;
using static Common.Core.Constants.Setting;
using NotificationType = Common.Core.Constants.Setting.NotificationType;

public class NotificationService : BaseS, INotificationService
{
    public NotificationService(IMcsgContext context, IHubContext<NotificationHub> hubcontext) : base(context)
    {
        _hubcontext = hubcontext;
    }

    public async Task<NotificationResponse> AddTransactionNotification(TransactionNotificationReq req)
    {
        var response = new NotificationResponse();

        var noti = new NotificationDto();

        NotificationEntityType notificationEntityType = req.TransactionType switch
        {
            TransactionType.Transfer => NotificationEntityType.TransferTransaction,
            TransactionType.Donate => NotificationEntityType.DonateTransaction,
            _ => NotificationEntityType.TransferTransaction
        };

        noti = await AddNotificationAsync(
                            actorId: req.AuthorId
                          , receiverId: req.ReceiverId
                          , action: NotificationAction.Transaction
                          , entityType: notificationEntityType
                          , entityId: req.Id);
        var userInfo = await _context.UserAvailable.Where(p => p.Id == req.AuthorId)
            .Select(p => new
            {
                ProfileName = p.ProfileName + "",
                UserAvatar = p.Avatar
            })
            .FirstOrDefaultAsync();
        var amount = req.Amount.ToString("N0");
        response.Id = noti.Id;
        response.Amount = amount;
        response.Status = noti.Status;
        response.EntityId = req.Id;
        response.ReferenceNumber = req.ReferenceNumber;
        response.ActorId = req.AuthorId;
        response.ActorName = userInfo.ProfileName;
        response.Message = GetMessageTransaction(amount, userInfo.ProfileName, notificationEntityType);
        response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
        response.NotificationType = GetTransactionType(notificationEntityType);
        response.UserAvatar = userInfo.UserAvatar;
        await _hubcontext.Clients.Group(req.ReceiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        return response;
    }

    public async Task SendSuccessCommentNotification(PostCommentResp comment, string microService)
    {
        var isPost = comment.Type == "post";
        var response = new NotificationSuccessResponse
        {
            CommentId = comment.Id,
            MicroService = microService,
            PostId = isPost ? comment.PostId : comment.PostIdOfPost,
            SubPostId = isPost ? null : comment.PostId
        };

        await _hubcontext.Clients.Group(comment.AuthorId.ToString()).SendAsync(RealTimeTopic.ReceiveSendSuccessComment, JsonConvert.SerializeObject(response));
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
                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }
        }

        return response;
    }

    private string GetTargetType(CommentNotificationReq comment)
    {
        switch (comment.PostType)
        {
            case PostType.Comic:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Comic : NotificationTargetType.SubComic;
            case PostType.Story:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Story : NotificationTargetType.SubStory;
            default:
                return comment.Type == PostTypes.Post ? NotificationTargetType.Social : NotificationTargetType.SubSocial;
        }
    }

    private string GetMessageTransaction(string amount, string profileName, NotificationEntityType notificationEntityType)
    {
        return notificationEntityType switch
        {
            NotificationEntityType.TransferTransaction => string.Format(NotificationContent.TransferTransaction, amount, profileName),
            NotificationEntityType.DonateTransaction => string.Format(NotificationContent.DonateTransaction, profileName),
        };
    }

    private string GetTransactionType(NotificationEntityType notificationEntityType)
    {
        return notificationEntityType switch
        {
            NotificationEntityType.TransferTransaction => NotificationType.TransferTransaction,
            NotificationEntityType.DonateTransaction => NotificationType.DonateTransaction
        };
    }

    private string GetMessage(CommentNotificationReq comment)
    {
        switch (comment.PostType)
        {
            case PostType.Comic:
                return comment.AuthorName + NotificationContent.CommentOnComic;

            case PostType.Story:
                return comment.AuthorName + NotificationContent.CommentOnStory;

            default:
                return comment.AuthorName + NotificationContent.CommentOnFeed;
        }
    }

    public async Task SendSuccessReplyNotification(ReplyCommentResp comment, string microService)
    {
        var isPost = comment.Type == "post";
        var response = new NotificationSuccessResponse
        {
            CommentId = comment.Id,
            MicroService = microService,
            PostId = isPost ? comment.PostId : comment.PostIdOfPost,
            SubPostId = isPost ? null : comment.PostId
        };

        await _hubcontext.Clients.Group(comment.AuthorId.ToString()).SendAsync(RealTimeTopic.ReceiveSendSuccessComment, JsonConvert.SerializeObject(response));
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
                qAuthorId = _context.ComicPostCommentAvailable.Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.ComicPostAvailable.Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubComic:
                qAuthorId = _context.ComicSubPostCommentAvailable.Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.ComicSubPostAvailable.Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.Social:
                qAuthorId = _context.SocialPostCommentAvailable.Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.SocialPostAvailable.Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubSocial:
                qAuthorId = _context.SocialSubPostCommentAvailable.Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.SocialSubPostAvailable.Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.Story:
                qAuthorId = _context.StoryPostCommentAvailable.Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.StoryPostAvailable.Where(p => p.Id == postId).Select(p => p.HashId);
                break;

            case NotificationTargetType.SubStory:
                qAuthorId = _context.StorySubPostCommentAvailable.Where(p => p.Id == id).Select(p => p.AuthorId);
                qHashId = _context.StorySubPostAvailable.Where(p => p.Id == postId).Select(p => p.HashId);
                break;
        }

        var authorId = await qAuthorId.FirstOrDefaultAsync();

        // Dont notify when comment on their feed
        if (authorId != Guid.Empty && authorId != comment.AuthorId)
        {
            var hashId = await qHashId.FirstOrDefaultAsync();

            var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: authorId
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
            response.Message = comment.AuthorName + NotificationContent.ReplyOnComment;
            response.TargetType = targetType;
            response.ActorId = comment.AuthorId;
            response.ActorName = comment.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.Reply;
            response.UserAvatar = comment.UserAvatar;
            response.Order = comment.Order ?? 0;
            response.ReplyCommentId = comment.Id;
            response.CommentId = comment.ReplyToCommentId;

            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(authorId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

        return response;
    }

    public async Task<NotificationResponse> AddVideoNotification(VideoNotificationR video)
    {
        var response = new NotificationResponse();

        var receiverId = video.AuthorId;
        var noti = await AddNotificationAsync(
                                    actorId: video.AuthorId
                                    , receiverId: receiverId
                                    , action: video.Action
                                    , entityType: NotificationEntityType.Video
                                    , entityId: video.Id
                                    , locationId: video.PostId
                                    , locationHashId: video.PostHashId);

        response.Id = noti.Id;
        response.Status = noti.Status;
        response.LocationId = video.PostId;
        response.LocationHashId = video.PostHashId;
        response.EntityHashId = video.HashId;
        response.Message = GetVideoMessage(video.Action);
        response.TargetType = !string.IsNullOrWhiteSpace(video.TargetType) ? video.TargetType : NotificationTargetType.None;
        response.ActorId = video.AuthorId;
        response.ActorName = video.AuthorName;
        response.UserAvatar = video.UserAvatar;
        response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
        response.NotificationType = NotificationType.Video + video.Action.ToString();

        // Then notification the comment to post owner
        await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

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
        string locationHashId = "";
        var targetId = reaction.TargetId;
        bool isCommentReaction = reaction.EntityType != NotificationEntityType.SocialPostReaction
            && reaction.EntityType != NotificationEntityType.StoryPostReaction
            && reaction.EntityType != NotificationEntityType.ComicPostReaction
            && reaction.EntityType != NotificationEntityType.SocialSubPostReaction;

        // Reaction for comment => Find Id of Post
        if (isCommentReaction)
        {
            var qComment = _context.SocialPostCommentAvailable.Where(p => p.Id == targetId)
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
                    qComment = _context.SocialPostCommentAvailable.Where(p => p.Id == targetId)
                        .Select(p => new
                        {
                            p.Id,
                            p.PostId,
                            p.AuthorId,
                            p.ParentId,
                        });
                    break;

                case NotificationEntityType.ComicPostCommentReaction or NotificationEntityType.ComicPostCommentReplyReaction:
                    qComment = _context.ComicPostCommentAvailable.Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.StoryPostCommentReaction or NotificationEntityType.StoryPostCommentReplyReaction:
                    qComment = _context.StoryPostCommentAvailable.Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.SocialSubPostCommentReaction or NotificationEntityType.SocialSubPostCommentReplyReaction:
                    qComment = _context.SocialSubPostCommentAvailable.Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.ComicSubPostCommentReaction or NotificationEntityType.SocialPostCommentReplyReaction:
                    qComment = _context.ComicSubPostCommentAvailable.Where(p => p.Id == targetId)
                    .Select(p => new
                    {
                        p.Id,
                        p.PostId,
                        p.AuthorId,
                        p.ParentId,
                    });
                    break;

                case NotificationEntityType.StorySubPostCommentReaction or NotificationEntityType.StorySubPostCommentReplyReaction:
                    qComment = _context.StorySubPostCommentAvailable.Where(p => p.Id == targetId)
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
            response.Message = reaction.AuthorName + NotificationContent.ReactOnComment;
        }

        var q = _context.SocialPostAvailable.Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0, PostId = Guid.Empty });
        switch (reaction.EntityType)
        {
            case NotificationEntityType.SocialPostReaction:
            case NotificationEntityType.SocialPostCommentReaction:
            case NotificationEntityType.SocialPostCommentReplyReaction:
                break;

            case NotificationEntityType.ComicPostReaction:
            case NotificationEntityType.ComicPostCommentReaction:
            case NotificationEntityType.ComicPostCommentReplyReaction:
                q = _context.ComicPostAvailable.Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0, PostId = Guid.Empty });
                break;

            case NotificationEntityType.StoryPostReaction:
            case NotificationEntityType.StoryPostCommentReaction:
            case NotificationEntityType.StoryPostCommentReplyReaction:
                q = _context.StoryPostAvailable.Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = 0, PostId = Guid.Empty });
                break;

            case NotificationEntityType.SocialSubPostReaction:
            case NotificationEntityType.SocialSubPostCommentReaction:
            case NotificationEntityType.SocialSubPostCommentReplyReaction:
                q = _context.SocialSubPostAvailable.Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, p.Order, p.PostId });
                break;

            case NotificationEntityType.ComicSubPostCommentReaction:
            case NotificationEntityType.ComicSubPostCommentReplyReaction:
                q = _context.ComicSubPostAvailable.Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = (int)p.Order, p.PostId });
                break;

            case NotificationEntityType.StorySubPostCommentReaction:
            case NotificationEntityType.StorySubPostCommentReplyReaction:
                q = _context.StorySubPostAvailable.Where(p => p.Id == targetId).Select(p => new { p.Id, p.HashId, p.UserId, Order = (int)p.Order, p.PostId });
                break;

            default:
                break;
        }

        var ett = await q.FirstOrDefaultAsync();

        if (ett != null)
        {
            postId = ett.Id;
            postHashId = ett.HashId;

            // Only reaction by subpost comment need order to go to subpost Comic/Story
            if (reaction.EntityType == NotificationEntityType.ComicSubPostCommentReaction || reaction.EntityType == NotificationEntityType.StorySubPostCommentReaction ||
                reaction.EntityType == NotificationEntityType.ComicSubPostCommentReplyReaction || reaction.EntityType == NotificationEntityType.StorySubPostCommentReplyReaction)
            {
                response.Order = ett.Order;
                Guid postIdOfSubPost = ett.PostId;
                IQueryable<string?> qHashId = default!;

                switch (reaction.EntityType)
                {
                    case NotificationEntityType.ComicSubPostCommentReaction:
                        qHashId = _context.ComicPostAvailable.Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.StorySubPostCommentReaction:
                        qHashId = _context.StoryPostAvailable.Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.ComicSubPostCommentReplyReaction:
                        qHashId = _context.ComicPostAvailable.Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    case NotificationEntityType.StorySubPostCommentReplyReaction:
                        qHashId = _context.ComicPostAvailable.Where(p => p.Id == postIdOfSubPost).Select(p => p.HashId);
                        break;

                    default:
                        break;
                }

                postHashId = await qHashId.FirstOrDefaultAsync();

            }

            if (!isCommentReaction)
            {
                receiverId = ett.UserId != null ? ett.UserId : Guid.Empty;
                locationId = ett.Id;
                locationHashId = ett.HashId;
                var message = reaction.EntityType == NotificationEntityType.SocialPostReaction ? NotificationContent.ReactOnFeed : NotificationContent.ReactOnComic;
                response.Message = reaction.AuthorName + message;
            }

            response.TargetType = reaction.EntityType switch
            {
                NotificationEntityType.SocialPostReaction or NotificationEntityType.SocialPostCommentReaction or NotificationEntityType.SocialPostCommentReplyReaction => NotificationTargetType.Social,
                NotificationEntityType.StoryPostReaction or NotificationEntityType.StoryPostCommentReaction or NotificationEntityType.StoryPostCommentReplyReaction => NotificationTargetType.Story,
                NotificationEntityType.ComicPostReaction or NotificationEntityType.ComicPostCommentReaction or NotificationEntityType.ComicPostCommentReplyReaction => NotificationTargetType.Comic,
                NotificationEntityType.SocialSubPostReaction or NotificationEntityType.SocialSubPostCommentReaction or NotificationEntityType.SocialSubPostCommentReplyReaction => NotificationTargetType.SubSocial,
                NotificationEntityType.ComicSubPostCommentReaction or NotificationEntityType.ComicSubPostCommentReplyReaction => NotificationTargetType.SubComic,
                NotificationEntityType.StorySubPostCommentReaction or NotificationEntityType.StorySubPostCommentReplyReaction => NotificationTargetType.SubStory,
            };

            var notificationObject = await _context.NotificationObjects.Where(p => p.EntityType == reaction.EntityType
                                                                                  && p.ActorId == reaction.AuthorId
                                                                                  && p.EntityId == reaction.Id).FirstOrDefaultAsync();
            if (notificationObject != null)
            {
                notificationObject.CreatedOn = DateTime.UtcNow;
                var notification = await _context.Notifications.FirstOrDefaultAsync(p => p.NotificationObjectId == notificationObject.Id);
                await _context.Notifications.Where(p => p.NotificationObjectId == notificationObject.Id).ExecuteUpdateAsync(p => p
                .SetProperty(x => x.Status, NotificationStatus.UnRead)
                .SetProperty(x => x.CreatedOn, DateTime.UtcNow));
                response.Id = notification.Id;
                response.Status = NotificationStatus.UnRead.ToString();
                response.LocationId = postId;
                response.LocationHashId = postHashId;
                response.EntityId = reaction.Id;
                response.ActorId = reaction.AuthorId;
                response.ActorName = reaction.AuthorName;
                response.CreatedOn = DateTime.UtcNow;
                response.NotificationType = NotificationType.Reaction;
                response.UserAvatar = reaction.UserAvatar;
                response.ReactionType = reaction.ReactionType;
                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
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
                                  , locationHashId: locationHashId);

                response.Id = noti.Id;
                response.Status = noti.Status;
                response.LocationId = postId;
                response.LocationHashId = postHashId;
                response.EntityId = reaction.Id;
                response.ActorId = reaction.AuthorId;
                response.ActorName = reaction.AuthorName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = NotificationType.Reaction;
                response.UserAvatar = reaction.UserAvatar;
                response.ReactionType = reaction.ReactionType;

                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }
        }

        return response;
    }

    public async Task AddPostMentionNotification(MentionPostNotificationReq request)
    {
        var response = new NotificationResponse();
        var receiverId = Guid.Empty;
        var postId = Guid.Empty;
        var locationId = Guid.Empty;
        string postHashId = "";
        string locationHashId = "";
        var targetId = request.TargetId;
        bool isMentionComment = request.EntityType != NotificationEntityType.SocialPostMention;
        // Reaction for comment => Find Id of Post

        if (isMentionComment)
        {
            dynamic postComment = request.EntityType switch
            {
                NotificationEntityType.SocialPostCommentMention => await _context.SocialPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId),
                NotificationEntityType.ComicPostCommentMention => await _context.ComicPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId),
                NotificationEntityType.StoryPostCommentMention => await _context.StoryPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId),
                NotificationEntityType.SocialSubPostCommentMention => await _context.SocialSubPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId),
                NotificationEntityType.ComicSubPostCommentMention => await _context.ComicSubPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId),
                NotificationEntityType.StorySubPostCommentMention => await _context.StorySubPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId),
                _ => await _context.SocialPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == request.TargetId)
            };

            targetId = postComment.PostId;
            locationId = request.TargetId;
            response.CommentId = request.TargetId;
        }

        dynamic post = request.EntityType switch
        {
            NotificationEntityType.SocialPostMention => await _context.SocialPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
            NotificationEntityType.ComicPostCommentMention => await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
            NotificationEntityType.StoryPostCommentMention => await _context.StoryPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
            NotificationEntityType.SocialSubPostCommentMention => await _context.SocialSubPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
            NotificationEntityType.ComicSubPostCommentMention => await _context.ComicSubPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
            NotificationEntityType.StorySubPostCommentMention => await _context.StorySubPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
            _ => await _context.SocialPostAvailable.FirstOrDefaultAsync(p => p.Id == targetId),
        };

        if (post != null)
        {
            postId = post.Id;
            postHashId = post.HashId;
            /// only reaction by subpost comment need order to go to subpost Comic/Story
            if (request.EntityType == NotificationEntityType.ComicSubPostCommentMention || request.EntityType == NotificationEntityType.StorySubPostCommentMention)
            {
                response.Order = post.Order;
                Guid postIdOfSubPost = post.PostId;
                postHashId = request.EntityType == NotificationEntityType.ComicSubPostCommentMention ? (await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.Id == postIdOfSubPost)).HashId : (await _context.StoryPostAvailable.FirstOrDefaultAsync(p => p.Id == postIdOfSubPost)).HashId;
            }
            if (!isMentionComment)
            {
                locationId = post.Id;
                locationHashId = post.HashId;
            }
            response.TargetType = request.EntityType switch
            {
                NotificationEntityType.SocialPostMention or NotificationEntityType.SocialPostCommentMention => NotificationTargetType.Social,
                NotificationEntityType.StoryPostCommentMention => NotificationTargetType.Story,
                NotificationEntityType.ComicPostCommentMention => NotificationTargetType.Comic,
                NotificationEntityType.SocialSubPostMention or NotificationEntityType.SocialSubPostCommentReaction => NotificationTargetType.SubSocial,
                NotificationEntityType.ComicSubPostCommentMention => NotificationTargetType.SubComic,
                NotificationEntityType.StorySubPostCommentMention => NotificationTargetType.SubStory,
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
                                  , locationHashId: locationHashId);

                response.Id = noti.Id;
                response.Status = noti.Status;
                response.LocationId = postId;
                response.LocationHashId = postHashId;
                response.EntityId = item;
                response.ActorId = request.UserId;
                response.UserAvatar = request.UserAvatar;
                response.ActorName = request.UserProfileName;
                response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                response.NotificationType = NotificationType.Mention;
                response.Message = request.UserProfileName + (isMentionComment ? NotificationContent.MentionOnComment : NotificationContent.MentionOnPost);
                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(item.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }
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
            response.Message = mention.AuthorName + NotificationContent.MentionOnComment;
            response.TargetType = targetType;
            response.ActorId = mention.AuthorId;
            response.ActorName = mention.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = NotificationType.Mention;
            response.UserAvatar = mention.UserAvatar;

            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
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
            Status = noti.Status.ToString(),
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
                Status = noti.Status.ToString(),
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
            await _hubcontext.Clients.Group(req.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveTransactionUpdate, JsonConvert.SerializeObject(req));
        }
    }

    public async Task AddCommonNotification(CommonNotificationReq req)
    {
        if (req != null && !string.IsNullOrEmpty(req.TopicName) && !string.IsNullOrEmpty(req.Message))
        {
            if (!string.IsNullOrEmpty(req.UserId))
            {
                await _hubcontext.Clients.Group(req.UserId).SendAsync(req.TopicName, req.Message);
            }
            else
            {
                await _hubcontext.Clients.All.SendAsync(req.TopicName, req.Message);
            }

        }
    }

    private string GetVideoMessage(NotificationAction action)
    {
        return action switch
        {
            NotificationAction.Processing => NotificationContent.VideoUploadProcessing,
            NotificationAction.Completed => NotificationContent.VideoUploadCompleted,
            NotificationAction.Failed => NotificationContent.VideoUploadFailed,
            _ => throw new NotSupportedException($"Unsupported video action: {action}"),
        };
    }

    public async Task<NotificationResponse> AddFollowPostNotification(FollowPostNotificationReq request)
    {
        var response = new NotificationResponse();
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
        response.Message = string.Format(NotificationContent.FollowPost, request.ActorName, request.PostName);
        response.TargetType = request.NotificationEntityType == NotificationEntityType.ComicPostFollow ? NotificationTargetType.Comic : NotificationTargetType.Story;
        response.ActorId = request.ActorId;
        response.ActorName = request.ActorName;
        response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
        response.NotificationType = NotificationType.FollowPost;
        response.UserAvatar = request.UserAvatar;
        await _hubcontext.Clients.Group(request.ReceiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        return response;
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

        var details = await _context.SocialReportDetailAvailable
            .Where(p => p.ReportId == request.EntityId && p.ModifiedBy == null)
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

            await _hubcontext.Clients.Group(i.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

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
                var notiObj = await (from a in _context.NotificationObjectAvailable
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
                    await _context.NotificationAvailable
                        .Where(p => p.NotificationObjectId == notiObj.Id)
                        .ExecuteUpdateAsync(setters => setters
                            .SetProperty(p => p.Status, p => NotificationStatus.UnRead)
                            .SetProperty(p => p.CreatedOn, p => DateTime.UtcNow)
                            .SetProperty(p => p.ModifiedOn, p => DateTime.UtcNow));
                    await _context.NotificationObjectAvailable
                        .Where(p => p.Id == notiObj.Id)
                        .ExecuteUpdateAsync(setters => setters
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
                    response.Message = followResp.CreatedByUserName + NotificationContent.FollowUser;
                    response.TargetType = NotificationTargetType.FollowUser;
                    response.ActorId = followResp.CreatedByUserId;
                    response.ActorName = followResp.CreatedByUserName;
                    response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                    response.NotificationType = NotificationType.FollowUser;
                    response.UserAvatar = followResp.CreatedByUserAvata;

                    await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
                    return response;
                }
            }
        }

        return response;
    }

    public async Task<NotificationResponse> AddDeletion(NotificationAddDeletionR request)
    {
        var response = new NotificationResponse();

        var q = _context.ComicPosts.AsNoTracking()
            .Where(p => p.Id == request.EntityId)
            .Select(p => new
            {
                p.ModifiedBy,
                p.UserId
            });

        var type = request.NotificationType.ToEnum(AddDeletionType.ComicPost);
        switch (type)
        {
            case AddDeletionType.ComicSubPost:
                q = _context.ComicSubPosts.AsNoTracking()
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       p.UserId
                   });
                break;

            case AddDeletionType.SocialPost:
                q = _context.SocialPosts.AsNoTracking()
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       p.UserId
                   });
                break;

            case AddDeletionType.StoryPost:
                q = _context.StoryPosts.AsNoTracking()
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       p.UserId
                   });
                break;

            case AddDeletionType.StorySubPost:
                q = _context.StorySubPosts.AsNoTracking()
                   .Where(p => p.Id == request.EntityId)
                   .Select(p => new
                   {
                       p.ModifiedBy,
                       p.UserId
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

        var action = (type == AddDeletionType.ComicSubPost || type == AddDeletionType.StorySubPost) ? NotificationAction.DeleteSubPost : NotificationAction.DeletePost;

        var targetType = type switch
        {
            AddDeletionType.StoryPost => NotificationTargetType.Story,
            AddDeletionType.ComicPost => NotificationTargetType.Comic,
            AddDeletionType.ComicSubPost => NotificationTargetType.SubComic,
            AddDeletionType.StorySubPost => NotificationTargetType.SubStory,
            _ => NotificationTargetType.Social
        };

        var message = type switch
        {
            AddDeletionType.ComicPost or AddDeletionType.StoryPost => nameof(S300),
            AddDeletionType.ComicSubPost or AddDeletionType.StorySubPost => nameof(S301),
            _ => nameof(S302)
        };

        var notiType = type switch
        {
            AddDeletionType.ComicPost or AddDeletionType.StoryPost => NotificationType.DeletePost,
            AddDeletionType.ComicSubPost or AddDeletionType.StorySubPost => NotificationType.DeleteSubPost,
            _ => NotificationType.DeleteSocial
        };

        var entityType = type switch
        {
            AddDeletionType.ComicPost => NotificationEntityType.ComicPostDelete,
            AddDeletionType.ComicSubPost => NotificationEntityType.ComicSubPostDelete,
            AddDeletionType.StoryPost => NotificationEntityType.StoryPostDelete,
            AddDeletionType.StorySubPost => NotificationEntityType.StorySubPostDelete,
            _ => NotificationEntityType.SocialPostDelete
        };

        var noti = await AddNotificationAsync(
                                actorId: ett.ModifiedBy!.Value
                                , receiverId: ett.UserId
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

        await _hubcontext.Clients.Group(ett.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

        return response;
    }

    public async Task<NotificationResponse> AddLock(NotificationAddLockR request)
    {
        var response = new NotificationResponse();

        var q = from report in _context.ComicReportAvailable.AsNoTracking()
                join post in _context.ComicPostAvailable.AsNoTracking() on report.EntityId equals post.Id
                where report.Id == request.EntityId
                select new
                {
                    report.Id,
                    report.EntityId,
                    report.ModifiedBy,
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
                q = from report in _context.ComicReportAvailable.AsNoTracking()
                    join post in _context.ComicSubPostAvailable.AsNoTracking() on report.EntityId equals post.Id
                    where report.Id == request.EntityId
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        report.ModifiedBy,
                        post.UserId
                    };

                action = NotificationAction.LockSubPost;
                targetType = NotificationTargetType.SubComic;
                message = nameof(S304);
                notiType = NotificationType.LockSubPost;
                entityType = NotificationEntityType.ComicSubPostLock;

                break;

            case AddLockType.SocialPost:
                q = from report in _context.SocialReportAvailable.AsNoTracking()
                    join post in _context.SocialPostAvailable.AsNoTracking() on report.EntityId equals post.Id
                    where report.Id == request.EntityId
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        report.ModifiedBy,
                        post.UserId
                    };

                action = NotificationAction.LockPost;
                targetType = NotificationTargetType.Social;
                message = nameof(S305);
                notiType = NotificationType.LockSocial;
                entityType = NotificationEntityType.SocialPostLock;

                break;

            case AddLockType.StoryPost:
                q = from report in _context.StoryReportAvailable.AsNoTracking()
                    join post in _context.StoryPostAvailable.AsNoTracking() on report.EntityId equals post.Id
                    where report.Id == request.EntityId
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        report.ModifiedBy,
                        post.UserId
                    };

                action = NotificationAction.LockPost;
                targetType = NotificationTargetType.Story;
                message = nameof(S303);
                notiType = NotificationType.LockPost;
                entityType = NotificationEntityType.StoryPostLock;

                break;

            case AddLockType.StorySubPost:
                q = from report in _context.StoryReportAvailable.AsNoTracking()
                    join post in _context.StorySubPostAvailable.AsNoTracking() on report.EntityId equals post.Id
                    where report.Id == request.EntityId
                    select new
                    {
                        report.Id,
                        report.EntityId,
                        report.ModifiedBy,
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
                                actorId: ett.ModifiedBy!.Value
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
        response.ActorId = ett.ModifiedBy.Value;
        response.CreatedOn = noti.CreatedOn;
        response.NotificationType = notiType;

        await _hubcontext.Clients.Group(ett.UserId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));

        return response;
    }

    #region -- Fields --

    private readonly IHubContext<NotificationHub> _hubcontext;

    #endregion
}
