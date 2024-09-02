using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Constants;
using Dtos;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class StoryCommentService : IStoryCommentService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<StoryPost> _postRepository;
    private readonly IRepository<StorySubPost> _subPostRepository;
    private readonly IRepository<StoryPostComment> _postCommentRepository;
    private readonly IRepository<StorySubPostComment> _subPostCommentRepository;
    private readonly IRepository<StoryResource> _resourceRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly IResourceCommentService _resourceCommentService;
    private readonly INotificationService _notificationService;
    private readonly IMentionService _mentionService;
    private readonly ISmartCountService _smartCountService;
    private readonly IBusinessText businessBodyText;
    private readonly IMapper _mapper;
    private IConfiguration _configuration;
    private readonly IMcsgContext _context;

    public StoryCommentService(ICurrentUserService currentUserService,
        IRepository<StoryPost> postRepository,
        IRepository<StorySubPost> subPostRepository,
        IRepository<StoryPostComment> postCommentRepository,
        IRepository<StorySubPostComment> subPostCommentRepository,
        IRepository<StoryResource> resourceRepository,
        IRepository<Mention> mentionRepository,
        IResourceCommentService resourceCommentService,
        INotificationService notificationService,
        IMentionService mentionService,
        ISmartCountService smartCountService,
        IBusinessText businessBodyText,
        IMapper mapper,
        ISetting setting,
        IConfiguration configuration,
        IMcsgContext context)
    {
        _currentUserService = currentUserService;
        _postRepository = postRepository;
        _subPostRepository = subPostRepository;
        _postCommentRepository = postCommentRepository;
        _subPostCommentRepository = subPostCommentRepository;
        _mentionRepository = mentionRepository;
        _resourceCommentService = resourceCommentService;
        _resourceRepository = resourceRepository;
        _notificationService = notificationService;
        _mentionService = mentionService;
        _smartCountService = smartCountService;
        _businessText = businessBodyText;
        _mapper = mapper;
        _setting = setting;
        _configuration = configuration;
        _context = context;
    }

    public async Task<PostCommentResp> PostComment(PostCommentReq req)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (!ValidComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }
        var response = new PostCommentResp();

        var payloadJson = user.Claims.FirstOrDefault(x => x.Type == Setting.Payload)?.Value ?? "";
        var payload = JsonConvert.DeserializeObject<Common.Core.Dtos.PayloadDto>(payloadJson);
        var receiverIds = req.CommentText.ToGuids();
        var userName = payload?.UserName;
        var profileName = payload?.ProfileName;
        var userFolder = payload?.UserFolder;
        var userAvatar = payload?.UserAvatar;

        var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
        var author = new AuthorDto() { Id = user.UserId.Value, Name = userName, Avatar = userAvatar };
        var rcDto = new ResourceCommentDto(userFolder, req.PostId, req.ResourceHashId, req.MicroService);
        var resource = await _resourceCommentService.AddResourceToComment(rcDto);
        var pDto = new PostDto();
        var order = 0.0f;

        if (req.Type == PostTypes.Post)
        {
            var post = await _postRepository.GetByIdAsync(req.PostId);
            if (post != null)
            {
                pDto.Id = post.Id;
                pDto.HashId = post.HashId;
                pDto.CreateBy = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;

                response = await CommentToPost(req, author, resource, pDto);
            }
        }
        else
        {
            var subPost = await _subPostRepository.GetByIdAsync(req.PostId);
            if (subPost != null)
            {
                order = subPost.Order;
                pDto.Id = subPost.Id;
                pDto.HashId = subPost.HashId;
                pDto.CreateBy = subPost.CreatedBy != null ? subPost.CreatedBy.Value : Guid.Empty;

                response = await CommentToSubPost(req, author, resource, pDto);
                response.Order = subPost.Order;
                response.PostIdOfPost = subPost.PostId;
            };
        }

        /// Check createdby in mention will not send this notification to notice that someone comment on their post
        if (!string.IsNullOrEmpty(pDto.HashId) && !receiverIds.Contains(pDto.CreateBy))
        {
            response.AuthorName = authorName;
            response.PostHashId = pDto.HashId;
            response.PostCreatedBy = pDto.CreateBy;



            // Send notification
            var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
            if (commentNotiRequest.Type == "subpost")
            {
                commentNotiRequest.Order = order;
                var post = await _postRepository.GetByIdAsync(response.PostIdOfPost);
                commentNotiRequest.PostHashId = post.HashId;
            }
            await _notificationService.AddCommentNotification(commentNotiRequest);
        }

        if (receiverIds.Count > 0)
        {
            await _notificationService.AddPostMentionNotification(new MentionPostNotificationReq
            {
                ReceiversId = receiverIds,
                UserAvatar = userAvatar,
                UserProfileName = profileName,
                TargetId = response.Id,
                UserId = user.UserId ?? Guid.Empty,
                EntityType = req.Type == PostTypes.Post ? NotificationEntityType.ComicPostCommentMention : NotificationEntityType.ComicSubPostCommentMention
            });
        }

        var userIds = response.Mentions.Select(p => p.EntityId).ToList();
        var userInfos = await _context.UserAvailable
            .AsNoTracking()
            .Where(p => userIds.Contains(p.Id))
            .Select(p => new
            {
                p.Id,
                p.UserName
            })
            .ToListAsync();
        var dicUser = userInfos.ToDictionary(p => p.Id, q => q.UserName);

        foreach (var i in response.Mentions)
        {
            i.ProfileName = i.Text.Substring(1);
            i.UserName = dicUser.GetValueOrDefault(i.EntityId);
        }

        response.UserAvatar = userAvatar;
        response.CommentText = await _businessText.Process(req.CommentText);
        response.CustomNote = req.CustomNote;

        return response;
    }
    public async Task<PostCommentResp> UpdateComment(UpdateCommentReq req)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (!ValidComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        var payloadJson = user.Claims.FirstOrDefault(x => x.Type == Setting.Payload)?.Value ?? "";
        var payload = JsonConvert.DeserializeObject<Common.Core.Dtos.PayloadDto>(payloadJson);

        var userName = payload?.UserName;
        var profileName = payload?.ProfileName;
        var userFolder = payload?.UserFolder;
        var userAvatar = payload?.UserAvatar;

        var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
        var author = new AuthorDto() { Id = user.UserId.Value, Name = userName, Avatar = userAvatar };
        var rcDto = new ResourceCommentDto(userFolder, req.PostId, req.ResourceHashId, req.MicroService);
        var resource = await _resourceCommentService.AddResourceToComment(rcDto);
        var pDto = new PostDto();
        var response = new PostCommentResp();

        if (req.Type == PostTypes.Post)
        {
            var post = await _postRepository.GetByIdAsync(req.PostId);
            if (post != null)
            {
                pDto.Id = post.Id;
                pDto.HashId = post.HashId;
                pDto.CreateBy = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;
                response = await UpdateCommentToPost(req, author, resource, pDto);
            }
        }
        else
        {
            var subPost = await _subPostRepository.GetByIdAsync(req.PostId);
            if (subPost != null)
            {
                pDto.Id = subPost.Id;
                pDto.HashId = subPost.HashId;
                pDto.CreateBy = subPost.CreatedBy != null ? subPost.CreatedBy.Value : Guid.Empty;
                response = await UpdateCommentToSubPost(req, author, resource, pDto);
            }
        }

        response.AuthorName = authorName;
        response.UserAvatar = userAvatar;
        response.CustomNote = req.CustomNote;

        return response;
    }
    public async Task<PostCommentResp> DeleteComment(DeleteCommentReq req)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (req.CommentId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        if (req.Type == PostTypes.Post)
        {
            return await DeleteCommentInPost(req, user.UserId.Value);
        }
        else
        {
            return await DeleteCommentInSubPost(req, user.UserId.Value);
        }
    }

    #region Add New Comment
    private async Task<PostCommentResp> CommentToPost(PostCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new StoryPostComment
        {
            AuthorId = author.Id,
            Body = req.CommentText,
            CreatedBy = author.Id,
            ModifiedBy = author.Id,
            PostId = req.PostId,
            Status = CommentStatus.Public,
            ResourceId = resource?.Id ?? null,
            GifId = req.GifId,
            CustomNote = req.CustomNote
        };

        await _postCommentRepository.InsertAsync(comment);
        //PING COUNT
        await _smartCountService.QueueAddCommentCount(req.PostId, EntityType.Post);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.ModifiedOn.Value,
            Type = PostTypes.Post,
            Id = comment.Id,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = req.GifId,
            Mentions = req.Mentions
        };
    }
    private async Task<PostCommentResp> CommentToSubPost(PostCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new StorySubPostComment
        {
            AuthorId = author.Id,
            Body = req.CommentText,
            CreatedBy = author.Id,
            ModifiedBy = author.Id,
            PostId = req.PostId,
            Status = CommentStatus.Public,
            ResourceId = resource?.Id ?? null,
            GifId = req.GifId,
            CustomNote = req.CustomNote
        };
        await _subPostCommentRepository.InsertAsync(comment);
        //PING COUNT
        await _smartCountService.QueueAddCommentCount(req.PostId, EntityType.SubPost);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.ModifiedOn.Value,
            Type = PostTypes.SubPost,
            Id = comment.Id,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = req.GifId,
            Mentions = req.Mentions
        };
    }
    #endregion

    #region Update
    private async Task<PostCommentResp> UpdateCommentToPost(UpdateCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = await _postCommentRepository.GetByIdAsync(req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != author.Id)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        comment.Body = req.CommentText;
        comment.ModifiedBy = author.Id;
        comment.ModifiedOn = DateTime.UtcNow;
        comment.ResourceId = resource?.Id ?? null;
        comment.GifId = req.GifId;
        comment.CustomNote = req.CustomNote;
        await _postCommentRepository.UpdateAsync(comment);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.ModifiedOn.Value,
            Type = PostTypes.Post,
            Id = comment.Id,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = comment.GifId,
            Mentions = req.Mentions
        };
    }
    private async Task<PostCommentResp> UpdateCommentToSubPost(UpdateCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = await _subPostCommentRepository.GetByIdAsync(req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != author.Id)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        comment.Body = req.CommentText;
        comment.ModifiedBy = author.Id;
        comment.ModifiedOn = DateTime.UtcNow;
        comment.ResourceId = resource?.Id ?? null;
        comment.GifId = req.GifId;
        comment.CustomNote = req.CustomNote;
        await _subPostCommentRepository.UpdateAsync(comment);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.ModifiedOn.Value,
            Type = PostTypes.SubPost,
            Id = comment.Id,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = comment.GifId,
            Mentions = req.Mentions
        };
    }
    #endregion

    #region Delete
    private async Task<PostCommentResp> DeleteCommentInPost(DeleteCommentReq req, Guid userId)
    {
        var comment = await _postCommentRepository.GetByIdAsync(req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != userId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var command = string.Format(DeleteCommentCommand, _postCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _postCommentRepository.Connection.ExecuteAsync(command,
                    new
                    {
                        Id = req.CommentId,
                        ModifiedBy = userId,
                        ModifiedOn = DateTime.UtcNow,
                        LocationType = (int)MentionLocationType.PostComment
                    });

        //PING COUNT
        await _smartCountService.QueueRemoveCommentCount(req.CommentId, EntityType.Post);
        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentDate = comment.ModifiedOn.Value,
            Type = PostTypes.Post,
            Id = comment.Id,
        };
    }
    private async Task<PostCommentResp> DeleteCommentInSubPost(DeleteCommentReq req, Guid userId)
    {
        var comment = await _subPostCommentRepository.GetByIdAsync(req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != userId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var command = string.Format(DeleteCommentCommand, _subPostCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _subPostCommentRepository.Connection.ExecuteAsync(command,
                        new
                        {
                            Id = req.CommentId,
                            ModifiedBy = userId,
                            ModifiedOn = DateTime.UtcNow,
                            LocationType = (int)MentionLocationType.SubPostComment
                        });

        //PING COUNT
        await _smartCountService.QueueRemoveCommentCount(req.CommentId, EntityType.SubPost);
        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentDate = comment.ModifiedOn.Value,
            Type = PostTypes.SubPost,
            Id = comment.Id,
        };
    }
    #endregion
    private bool ValidComment(PostCommentReq req)
    {
        if (req.PostId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidPostId, RealtimeErrorMessage.InvalidPostId);
        }
        if (string.IsNullOrWhiteSpace(req.CommentText + req.ResourceHashId + req.GifId))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidMessage, RealtimeErrorMessage.InvalidMessage);
        }
        return true;
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    #endregion
}
