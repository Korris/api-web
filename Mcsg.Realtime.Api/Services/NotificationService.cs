using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Npgsql;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Enums;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Constants;
using Dtos;
using Hubs;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Models.RealTime;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Requests;
using static Mcsg.Common.Core.Constants.Setting;

public class NotificationService : INotificationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<SocialSubPost> _subPostRepository;
    private readonly IRepository<ComicPost> _comicPostRepository;
    private readonly IRepository<StoryPost> _storyPostRepository;
    private readonly IRepository<SocialPostComment> _postCommentRepository;
    private readonly IRepository<ComicPostComment> _comicPostCommentRepository;
    private readonly IRepository<StoryPostComment> _storyPostCommentRepository;
    private readonly IRepository<SocialSubPostComment> _subPostCommentRepository;
    private readonly IRepository<ComicSubPostComment> _comicSubPostCommentRepository;
    private readonly IRepository<StorySubPostComment> _storySubPostCommentRepository;
    private readonly IRepository<ComicSubPost> _comicSubPostRepository;
    private readonly IRepository<StorySubPost> _storySubPostRepository;
    private readonly IRepository<Notification> _notiRepository;
    private readonly IRepository<NotificationObject> _notiObjectRepository;
    private readonly IHubContext<NotificationHub> _hubcontext;
    private IUnitOfWork _unitOfWork;
    private readonly IMcsgContext _context;

    public NotificationService(ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        IHubContext<NotificationHub> hubcontext, IMcsgContext context,
        IRepository<ComicPost> comicPostRepository,
        IRepository<StoryPost> storyPostRepository,
        IRepository<SocialPostComment> postCommentRepository,
        IRepository<ComicPostComment> comicPostCommentRepository,
        IRepository<StoryPostComment> storyPostCommentRepository,
        IRepository<ComicSubPostComment> comicSubPostCommentRepository,
        IRepository<StorySubPostComment> storySubPostCommentRepository,
        IRepository<SocialSubPost> subPostRepository)
    {
        _currentUserService = currentUserService;
        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _notiRepository = unitOfWork.GetRepository<Notification>();
        _notiObjectRepository = unitOfWork.GetRepository<NotificationObject>();
        _postCommentRepository = unitOfWork.GetRepository<SocialPostComment>();
        _subPostCommentRepository = unitOfWork.GetRepository<SocialSubPostComment>();
        _comicSubPostRepository = unitOfWork.GetRepository<ComicSubPost>();
        _storySubPostRepository = unitOfWork.GetRepository<StorySubPost>();
        _unitOfWork = unitOfWork;
        _hubcontext = hubcontext;
        _context = context;
        _comicPostRepository = comicPostRepository;
        _storyPostRepository = storyPostRepository;
        _postCommentRepository = postCommentRepository;
        _comicPostCommentRepository = comicPostCommentRepository;
        _storyPostCommentRepository = storyPostCommentRepository;
        _comicSubPostCommentRepository = comicSubPostCommentRepository;
        _storySubPostCommentRepository = storySubPostCommentRepository;
        _subPostRepository = subPostRepository;
    }

    public async Task<NotificationResponse> AddCommentNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();

        //var post = await _postRepository.GetByIdAsync(comment.PostId);
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
                response.NotificationType = Common.Core.Constants.Setting.NotificationType.Comment;
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
                return comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.Comic : Common.Core.Constants.Setting.NotificationTargetType.SubComic;
            case PostType.Story:
                return comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.Story : Common.Core.Constants.Setting.NotificationTargetType.SubStory;
            default:
                return comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.Feed : Common.Core.Constants.Setting.NotificationTargetType.SubFeed;
        }
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
                ;

        }
    }

    public async Task<NotificationResponse> AddReplyNotification(CommentNotificationReq comment)
    {
        var response = new NotificationResponse();
        var receiverId = Guid.Empty;

        if (comment.Type == PostTypes.Post)
        {
            var parentComment = await _postCommentRepository.GetByIdAsync(comment.ReplyToCommentId.Value);
            receiverId = parentComment.AuthorId;
        }
        else
        {
            var parentComment = await _subPostCommentRepository.GetByIdAsync(comment.ReplyToCommentId.Value);
            receiverId = parentComment.AuthorId;
        }

        // Dont notify when comment on their feed
        if (receiverId != Guid.Empty && receiverId != comment.AuthorId)
        {
            var post = await _postRepository.GetByIdAsync(comment.PostId);

            var noti = await AddNotificationAsync(
                                        actorId: comment.AuthorId
                                        , receiverId: receiverId
                                        , action: comment.IsReply ? NotificationAction.Reply : NotificationAction.Comment
                                        , entityType: comment.EntityType
                                        , entityId: comment.Id
                                        , locationId: comment.PostId
                                        , locationHashId: post.HashId);

            response.Id = noti.Id;
            response.Status = noti.Status;
            response.LocationId = comment.PostId;
            response.LocationHashId = post.HashId;
            response.EntityId = comment.Id;
            response.Message = comment.AuthorName + NotificationContent.ReplyOnComment;
            response.TargetType = comment.Type == PostTypes.Post ? Common.Core.Constants.Setting.NotificationTargetType.ReplyOnFeed : Common.Core.Constants.Setting.NotificationTargetType.ReplyOnSubFeed;
            response.ActorId = comment.AuthorId;
            response.ActorName = comment.AuthorName;
            response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
            response.NotificationType = Common.Core.Constants.Setting.NotificationType.Reply;
            response.UserAvatar = comment.UserAvatar;

            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
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
        response.TargetType = !string.IsNullOrWhiteSpace(video.TargetType) ? video.TargetType : Common.Core.Constants.Setting.NotificationTargetType.None;
        response.ActorId = video.AuthorId;
        response.ActorName = video.AuthorName;
        response.UserAvatar = video.UserAvatar;
        response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
        response.NotificationType = Common.Core.Constants.Setting.NotificationType.Video + video.Action.ToString();

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
        string postHashId = "";
        string locationHashId = "";
        var targetId = reaction.TargetId;
        bool isCommentReaction = reaction.EntityType != NotificationEntityType.PostReaction
            && reaction.EntityType != NotificationEntityType.StoryPostReaction
            && reaction.EntityType != NotificationEntityType.ComicPostReaction
            && reaction.EntityType != NotificationEntityType.SubPostReaction;
        // Reaction for comment => Find Id of Post
        if (isCommentReaction)
        {
            dynamic postComment = reaction.EntityType switch
            {
                NotificationEntityType.PostCommentReaction => await _postCommentRepository.GetByIdAsync(reaction.TargetId),
                NotificationEntityType.ComicPostCommentReaction => await _comicPostCommentRepository.GetByIdAsync(reaction.TargetId),
                NotificationEntityType.StoryPostCommentReaction => await _storyPostCommentRepository.GetByIdAsync(reaction.TargetId),
                NotificationEntityType.SubPostCommentReaction => await _subPostCommentRepository.GetByIdAsync(reaction.TargetId),
                NotificationEntityType.ComicSubPostCommentReaction => await _comicSubPostCommentRepository.GetByIdAsync(reaction.TargetId),
                NotificationEntityType.StorySubPostCommentReaction => await _storySubPostCommentRepository.GetByIdAsync(reaction.TargetId),
                _ => await _postCommentRepository.GetByIdAsync(reaction.TargetId)
            };

            targetId = postComment.PostId;
            receiverId = postComment.CreatedBy;
            //locationHashId = postComment.HashId;
            locationId = reaction.TargetId;
            response.CommentId = reaction.TargetId;
            response.Message = reaction.AuthorName + NotificationContent.ReactOnComment;
        }

        dynamic post = reaction.EntityType switch
        {
            NotificationEntityType.PostReaction or NotificationEntityType.PostCommentReaction => await _postRepository.GetByIdAsync(targetId),
            NotificationEntityType.ComicPostReaction or NotificationEntityType.ComicPostCommentReaction => await _comicPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.StoryPostReaction or NotificationEntityType.StoryPostCommentReaction => await _storyPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.SubPostReaction or NotificationEntityType.SubPostCommentReaction => await _subPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.ComicSubPostCommentReaction => await _comicSubPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.StorySubPostCommentReaction => await _storySubPostRepository.GetByIdAsync(targetId),
            _ => await _postRepository.GetByIdAsync(targetId),
        };

        if (post != null)
        {
            postId = post.Id;
            postHashId = post.HashId;
            /// only reaction by subpost comment need order to go to subpost Comic/Story
            if (reaction.EntityType == NotificationEntityType.ComicSubPostCommentReaction || reaction.EntityType == NotificationEntityType.StorySubPostCommentReaction)
            {
                response.Order = post.Order;
                Guid postIdOfSubPost = post.PostId;
                postHashId = reaction.EntityType == NotificationEntityType.ComicSubPostCommentReaction ? (await _comicPostRepository.GetByIdAsync(postIdOfSubPost)).HashId : (await _storyPostRepository.GetByIdAsync(postIdOfSubPost)).HashId;
            }
            if (!isCommentReaction)
            {
                receiverId = post.UserId != null ? post.UserId : Guid.Empty;
                locationId = post.Id;
                locationHashId = post.HashId;
                response.Message = reaction.AuthorName + NotificationContent.ReactOnFeed;
            }
            response.TargetType = reaction.EntityType switch
            {
                NotificationEntityType.PostReaction or NotificationEntityType.PostCommentReaction => NotificationTargetType.Feed,
                NotificationEntityType.StoryPostReaction or NotificationEntityType.StoryPostCommentReaction => NotificationTargetType.Story,
                NotificationEntityType.ComicPostReaction or NotificationEntityType.ComicPostCommentReaction => NotificationTargetType.Comic,
                NotificationEntityType.SubPostReaction or NotificationEntityType.SubPostCommentReaction => NotificationTargetType.SubFeed,
                NotificationEntityType.ComicSubPostCommentReaction => NotificationTargetType.SubComic,
                NotificationEntityType.StorySubPostCommentReaction => NotificationTargetType.SubStory,
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
                response.NotificationType = Common.Core.Constants.Setting.NotificationType.Reaction;
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
                response.NotificationType = Common.Core.Constants.Setting.NotificationType.Reaction;
                response.UserAvatar = reaction.UserAvatar;
                response.ReactionType = reaction.ReactionType;
                // Then notification the comment to post owner
                await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
            }

            // React to subpost : TODO

            // React to comment : TODO

            // React to reply : TODO

            // Dont notify when comment on their feed

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
        bool isMentionComment = request.EntityType != NotificationEntityType.PostMention;
        // Reaction for comment => Find Id of Post

        if (isMentionComment)
        {
            dynamic postComment = request.EntityType switch
            {
                NotificationEntityType.PostCommentMention => await _postCommentRepository.GetByIdAsync(request.TargetId),
                NotificationEntityType.ComicPostCommentMention => await _comicPostCommentRepository.GetByIdAsync(request.TargetId),
                NotificationEntityType.StoryPostCommentMention => await _storyPostCommentRepository.GetByIdAsync(request.TargetId),
                NotificationEntityType.SubPostCommentMention => await _subPostCommentRepository.GetByIdAsync(request.TargetId),
                NotificationEntityType.ComicSubPostCommentMention => await _comicSubPostCommentRepository.GetByIdAsync(request.TargetId),
                NotificationEntityType.StorySubPostCommentMention => await _storySubPostCommentRepository.GetByIdAsync(request.TargetId),
                _ => await _postCommentRepository.GetByIdAsync(request.TargetId)
            };

            targetId = postComment.PostId;
            locationId = request.TargetId;
            response.CommentId = request.TargetId;
        }

        dynamic post = request.EntityType switch
        {
            NotificationEntityType.PostMention => await _postRepository.GetByIdAsync(targetId),
            NotificationEntityType.ComicPostCommentMention => await _comicPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.StoryPostCommentMention => await _storyPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.SubPostCommentMention => await _subPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.ComicSubPostCommentMention => await _comicSubPostRepository.GetByIdAsync(targetId),
            NotificationEntityType.StorySubPostCommentMention => await _storySubPostRepository.GetByIdAsync(targetId),
            _ => await _postRepository.GetByIdAsync(targetId),
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
                postHashId = request.EntityType == NotificationEntityType.ComicSubPostCommentMention ? (await _comicPostRepository.GetByIdAsync(postIdOfSubPost)).HashId : (await _storyPostRepository.GetByIdAsync(postIdOfSubPost)).HashId;
            }
            if (!isMentionComment)
            {
                locationId = post.Id;
                locationHashId = post.HashId;
            }
            response.TargetType = request.EntityType switch
            {
                NotificationEntityType.PostMention or NotificationEntityType.PostCommentMention => NotificationTargetType.Feed,
                NotificationEntityType.StoryPostCommentMention => NotificationTargetType.Story,
                NotificationEntityType.ComicPostCommentMention => NotificationTargetType.Comic,
                NotificationEntityType.SubPostMention or NotificationEntityType.SubPostCommentReaction => NotificationTargetType.SubFeed,
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
                response.NotificationType = Common.Core.Constants.Setting.NotificationType.Mention;
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
            var entityType = NotificationEntityType.PostCommentMention;
            var targetType = Common.Core.Constants.Setting.NotificationTargetType.CommentOnFeed;
            if (mention.LocationType == MentionLocationType.SubPostComment || mention.LocationType == MentionLocationType.SubPostCommentReply)
            {
                entityType = NotificationEntityType.SubPostCommentMention;
                targetType = Common.Core.Constants.Setting.NotificationTargetType.CommentOnSubFeed;
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
            response.NotificationType = Common.Core.Constants.Setting.NotificationType.Mention;
            response.UserAvatar = mention.UserAvatar;

            // Then notification the comment to post owner
            await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
        }

        return response;
    }

    public async Task<NotificationDto> AddNotificationAsync(Guid actorId, Guid receiverId, NotificationAction action
        , NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead
        , Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        try
        {
            var notiObj = new NotificationObject()
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

            await _notiObjectRepository.InsertAsync(notiObj);

            var dto = new NotificationDto();

            var noti = new Notification()
            {
                NotificationObjectId = notiObj.Id,
                ReceiverId = receiverId,
                Status = status
            };
            await _notiRepository.InsertAsync(noti);

            dto.Id = noti.Id;
            dto.Status = ((NotificationStatus)noti.Status).ToString();
            dto.NotificationObjectId = notiObj.Id;
            dto.ReceiverId = receiverId;
            dto.ActorId = actorId;
            dto.CreatedOn = noti.CreatedOn;

            return dto;

        }
        catch (PostgresException ex)
        {
            _unitOfWork.RollbackTransaction();
            throw ex;
        }
    }
    public async Task<List<NotificationDto>> AddNotificationsAsync(Guid actorId, List<Guid> receiverIds, NotificationAction action
        , NotificationEntityType entityType, NotificationStatus status = NotificationStatus.UnRead
        , Guid? entityId = null, Guid? locationId = null, string locationHashId = "", string entityhashId = "")
    {
        try
        {
            var response = new List<NotificationDto>();

            var notiObj = new NotificationObject()
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

            await _notiObjectRepository.InsertAsync(notiObj);

            foreach (var receiverId in receiverIds)
            {
                var dto = new NotificationDto();

                var noti = new Notification()
                {
                    NotificationObjectId = notiObj.Id,
                    ReceiverId = receiverId,
                    Status = status
                };
                await _notiRepository.InsertAsync(noti);

                dto.Id = noti.Id;
                dto.Status = ((NotificationStatus)noti.Status).ToString();
                dto.NotificationObjectId = notiObj.Id;
                dto.ReceiverId = receiverId;
                dto.ActorId = actorId;
                dto.CreatedOn = noti.CreatedOn;

                response.Add(dto);
            }

            return response;

        }
        catch (PostgresException ex)
        {
            _unitOfWork.RollbackTransaction();
            throw ex;
        }
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
        response.TargetType = request.NotificationEntityType == NotificationEntityType.FollowComicPost ? NotificationTargetType.Comic : NotificationTargetType.Story;
        response.ActorId = request.ActorId;
        response.ActorName = request.ActorName;
        response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
        response.NotificationType = Common.Core.Constants.Setting.NotificationType.FollowPost;
        response.UserAvatar = request.UserAvatar;
        await _hubcontext.Clients.Group(request.ReceiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
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
                                        , locationHashId: followResp.CreatedByUserName);

                    response.Id = noti.Id;
                    response.Status = noti.Status;
                    response.LocationId = null;
                    response.LocationHashId = followResp.CreatedByUserName;
                    response.EntityId = null;
                    response.Message = followResp.CreatedByUserName + NotificationContent.FollowUser;
                    response.TargetType = Common.Core.Constants.Setting.NotificationTargetType.FollowUser;
                    response.ActorId = followResp.CreatedByUserId;
                    response.ActorName = followResp.CreatedByUserName;
                    response.CreatedOn = noti?.CreatedOn ?? DateTime.UtcNow;
                    response.NotificationType = Common.Core.Constants.Setting.NotificationType.FollowUser;
                    response.UserAvatar = followResp.CreatedByUserAvata;

                    await _hubcontext.Clients.Group(receiverId.ToString()).SendAsync(RealTimeTopic.ReceiveNotification, JsonConvert.SerializeObject(response));
                    return response;
                }
            }
        }

        return response;
    }
}
