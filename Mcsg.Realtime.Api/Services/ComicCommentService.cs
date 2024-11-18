using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Extensions;
using Constants;
using Dtos;
using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Error;

public partial class ComicCommentService : BaseS, IComicCommentService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="businessText"></param>
    /// <param name="postCommentRepository"></param>
    /// <param name="subPostCommentRepository"></param>
    /// <param name="resourceRepository"></param>
    /// <param name="mentionRepository"></param>
    /// <param name="resourceCommentService"></param>
    /// <param name="notificationService"></param>
    /// <param name="mentionService"></param>
    /// <param name="smartCountService"></param>
    /// <param name="mapper"></param>
    public ComicCommentService(IMcsgContext context, IBusinessText businessText, IRepository<ComicPostComment> postCommentRepository, IRepository<ComicSubPostComment> subPostCommentRepository, IRepository<ComicResource> resourceRepository, IRepository<Mention> mentionRepository, IResourceCommentService resourceCommentService, INotificationService notificationService, IMentionService mentionService, ISmartCountService smartCountService, IMapper mapper) : base(context)
    {
        _postCommentRepository = postCommentRepository;
        _subPostCommentRepository = subPostCommentRepository;
        _mentionRepository = mentionRepository;
        _resourceCommentService = resourceCommentService;
        _resourceRepository = resourceRepository;
        _notificationService = notificationService;
        _mentionService = mentionService;
        _smartCountService = smartCountService;
        _businessText = businessText;
        _mapper = mapper;
    }

    public async Task<PostCommentResp> PostComment(PostCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        if (!ValidComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        var response = new PostCommentResp();

        var receiverIds = req.CommentText.ToGuids();
        var userName = req.UserName;
        var profileName = req.ProfileName;
        var userFolder = req.UserFolder;
        var userAvatar = req.UserAvatar;

        var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
        var author = new AuthorDto() { Id = userId.Value, Name = userName, Avatar = userAvatar };
        var rcDto = new ResourceCommentDto(userFolder, req.PostId, req.ResourceHashId, req.MicroService);
        var resource = await _resourceCommentService.AddResourceToComment(rcDto);
        var pDto = new PostDto();
        var order = 0.0f;
        req.CommentText = req.CommentText.RemoveMaliciousText();
        req.CustomNote = req.CustomNote.RemoveMaliciousText();

        if (req.Type == PostTypes.Post)
        {
            var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.Id == req.PostId);
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
            var subPost = await _context.ComicSubPostAvailable.FirstOrDefaultAsync(p => p.Id == req.PostId);
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

        response.PostType = PostType.Comic;
        /// Check createdby in mention will not send this notification to notice that someone comment on their post
        if (!string.IsNullOrEmpty(pDto.HashId) && !receiverIds.Contains(pDto.CreateBy))
        {
            response.AuthorName = authorName;
            response.PostHashId = pDto.HashId;
            response.PostCreatedBy = pDto.CreateBy;
            response.UserAvatar = userAvatar;

            // Send notification
            var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
            if (commentNotiRequest.Type == "subpost")
            {
                commentNotiRequest.Order = order;
                var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.Id == response.PostIdOfPost);
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
                UserId = userId.Value,
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
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        if (!ValidComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        #region -- Validate on server --
        // Commnent
        var ett = req.Type == "post"
                ? await _context.ComicPostCommentAvailable
                        .Where(p => p.Id == req.CommentId)
                        .Select(p => new { p.CreatedBy })
                        .FirstOrDefaultAsync()
                : await _context.ComicSubPostCommentAvailable
                        .Where(p => p.Id == req.CommentId)
                        .Select(p => new { p.CreatedBy })
                        .FirstOrDefaultAsync();

        if (ett == null)
        {
            throw new NotFoundException(nameof(E204), E204);
        }
        if (ett.CreatedBy != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        #endregion

        req.CommentText = req.CommentText.RemoveMaliciousText();
        req.CustomNote = req.CustomNote.RemoveMaliciousText();
        var userName = req.UserName;
        var profileName = req.ProfileName;
        var userFolder = req.UserFolder;
        var userAvatar = req.UserAvatar;

        var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
        var author = new AuthorDto() { Id = userId.Value, Name = userName, Avatar = userAvatar };
        var rcDto = new ResourceCommentDto(userFolder, req.PostId, req.ResourceHashId, req.MicroService);
        var resource = await _resourceCommentService.AddResourceToComment(rcDto);
        var pDto = new PostDto();
        var response = new PostCommentResp();

        if (req.Type == PostTypes.Post)
        {
            var post = await _context.ComicPostAvailable.FirstOrDefaultAsync(p => p.Id == req.PostId);
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
            var subPost = await _context.ComicSubPostAvailable.FirstOrDefaultAsync(p => p.Id == req.PostId);
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
        response.CommentText = await _businessText.Process(req.CommentText);

        return response;
    }

    public async Task<PostCommentResp> DeleteComment(DeleteCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        if (req.CommentId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        if (req.Type == PostTypes.Post)
        {
            return await DeleteCommentInPost(req);
        }
        else
        {
            return await DeleteCommentInSubPost(req);
        }
    }

    #region Add New Comment
    private async Task<PostCommentResp> CommentToPost(PostCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new ComicPostComment
        {
            AuthorId = author.Id,
            Body = req.CommentText.RemoveMaliciousText(),
            CreatedBy = author.Id,
            ModifiedBy = author.Id,
            PostId = req.PostId,
            Status = CommentStatus.Public,
            ResourceId = resource?.Id ?? null,
            GifId = req.GifId,
            CustomNote = req.CustomNote.RemoveMaliciousText()
        };
        await _context.ComicPostComments.AddAsync(comment);
        await _context.SaveChangesAsync(default);

        await _smartCountService.QueueAddCommentCount(req.PostId, EntityType.Post);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.CreatedOn,
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
        var comment = new ComicSubPostComment
        {
            AuthorId = author.Id,
            Body = req.CommentText.RemoveMaliciousText(),
            CreatedBy = author.Id,
            ModifiedBy = author.Id,
            PostId = req.PostId,
            Status = CommentStatus.Public,
            ResourceId = resource?.Id ?? null,
            GifId = req.GifId,
            CustomNote = req.CustomNote.RemoveMaliciousText()
        };
        await _context.ComicSubPostComments.AddAsync(comment);
        await _context.SaveChangesAsync(default);

        await _smartCountService.QueueAddCommentCount(req.PostId, EntityType.SubPost);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.CreatedOn,
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
        var comment = await _context.ComicPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != author.Id)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        comment.Body = req.CommentText.RemoveMaliciousText();
        comment.ModifiedBy = author.Id;
        comment.ModifiedOn = DateTime.UtcNow;
        comment.ResourceId = resource?.Id ?? null;
        comment.GifId = req.GifId;
        comment.CustomNote = req.CustomNote.RemoveMaliciousText();
        await _context.SaveChangesAsync(default);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.CreatedOn,
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
        var comment = await _context.ComicSubPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != author.Id)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        comment.Body = req.CommentText.RemoveMaliciousText();
        comment.ModifiedBy = author.Id;
        comment.ModifiedOn = DateTime.UtcNow;
        comment.ResourceId = resource?.Id ?? null;
        comment.GifId = req.GifId;
        comment.CustomNote = req.CustomNote.RemoveMaliciousText();
        await _context.SaveChangesAsync(default);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostComment, author, req.Mentions, post);

        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentText = comment.Body,
            CommentDate = comment.CreatedOn,
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
    private async Task<PostCommentResp> DeleteCommentInPost(DeleteCommentReq req)
    {
        var comment = await _context.ComicPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != req.UserId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var command = string.Format(DeleteCommentCommand, _postCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _postCommentRepository.Connection.ExecuteAsync(command,
                    new
                    {
                        Id = req.CommentId,
                        ModifiedBy = req.UserId,
                        ModifiedOn = DateTime.UtcNow,
                        LocationType = (int)MentionLocationType.PostComment
                    });

        //PING COUNT
        await _smartCountService.QueueRemoveCommentCount(req.CommentId, EntityType.Post);
        return new PostCommentResp
        {
            PostId = comment.PostId,
            CommentDate = comment.CreatedOn,
            Type = PostTypes.Post,
            Id = comment.Id,
        };
    }

    private async Task<PostCommentResp> DeleteCommentInSubPost(DeleteCommentReq req)
    {
        var comment = await _context.ComicSubPostCommentAvailable.FirstOrDefaultAsync(p => p.Id == req.CommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != req.UserId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }
        var post = await _context.ComicSubPostAvailable.FirstOrDefaultAsync(p => p.Id == comment.PostId);

        var command = string.Format(DeleteCommentCommand, _subPostCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _subPostCommentRepository.Connection.ExecuteAsync(command,
                        new
                        {
                            Id = req.CommentId,
                            ModifiedBy = req.UserId,
                            ModifiedOn = DateTime.UtcNow,
                            LocationType = (int)MentionLocationType.SubPostComment
                        });

        //PING COUNT
        await _smartCountService.QueueRemoveCommentCount(req.CommentId, EntityType.SubPost);
        return new PostCommentResp
        {
            PostIdOfPost = post.PostId,
            PostId = comment.PostId,
            CommentDate = comment.CreatedOn,
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
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    private readonly IRepository<ComicPostComment> _postCommentRepository;
    private readonly IRepository<ComicSubPostComment> _subPostCommentRepository;
    private readonly IRepository<ComicResource> _resourceRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly IResourceCommentService _resourceCommentService;
    private readonly INotificationService _notificationService;
    private readonly IMentionService _mentionService;
    private readonly ISmartCountService _smartCountService;
    private readonly IMapper _mapper;

    #endregion
}
