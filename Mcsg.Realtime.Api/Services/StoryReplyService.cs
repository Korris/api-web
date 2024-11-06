using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Constants;
using Dtos;
using Interfaces;
using Lib.Data.Repositories;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class StoryReplyService : IStoryReplyService
{
    private readonly IRepository<StoryPost> _postRepository;
    private readonly IRepository<StorySubPost> _subPostRepository;
    private readonly IRepository<StoryPostComment> _postCommentRepository;
    private readonly IRepository<StorySubPostComment> _subPostCommentRepository;
    private readonly IResourceCommentService _resourceCommentService;
    private readonly IRepository<StoryResource> _resourceRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly IStoryNotificationService _notificationService;
    private readonly IMentionService _mentionService;
    private readonly IMapper _mapper;
    private IConfiguration _configuration;
    private readonly IMcsgContext _context;

    public StoryReplyService(IRepository<StoryPost> postRepository,
        IRepository<StorySubPost> subPostRepository,
        IRepository<StoryPostComment> postCommentRepository,
        IRepository<StorySubPostComment> subPostCommentRepository,
        IResourceCommentService resourceCommentService,
        IRepository<StoryResource> resourceRepository,
        IRepository<Mention> mentionRepository,
        IStoryNotificationService notificationService,
        IBusinessText businessText,
        IMentionService mentionService,
        IMapper mapper,
        ISetting setting,
        IConfiguration configuration,
        IMcsgContext context)
    {
        _postRepository = postRepository;
        _subPostRepository = subPostRepository;
        _postCommentRepository = postCommentRepository;
        _subPostCommentRepository = subPostCommentRepository;
        _resourceCommentService = resourceCommentService;
        _resourceRepository = resourceRepository;
        _mentionRepository = mentionRepository;
        _notificationService = notificationService;
        _mentionService = mentionService;
        _mapper = mapper;
        _setting = setting;
        _configuration = configuration;
        _businessText = businessText;
        _context = context;
    }

    public async Task<ReplyCommentResp> ReplyComment(ReplyCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var response = new ReplyCommentResp();

        if (!ValidReplyComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        req.ReplyText = req.ReplyText.RemoveMaliciousText();
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

        if (req.Type == PostTypes.Post)
        {
            var post = await _postRepository.GetByIdAsync(req.PostId);
            if (post != null)
            {
                pDto.Id = post.Id;
                pDto.HashId = post.HashId;
                pDto.CreateBy = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;

                response = await ReplyToPostComment(req, author, resource, pDto);
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

                response = await ReplyToSubPostComment(req, author, resource, pDto);
                response.PostIdOfPost = subPost.PostId;
            };
        }

        if (!string.IsNullOrWhiteSpace(pDto.HashId))
        {
            response.AuthorName = authorName;
            response.UserAvatar = userAvatar;
            response.UserName = userName;
            response.QuoteId = req.QuoteId;
            // Send notification
            response.PostType = PostType.Story;
            var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
            if (commentNotiRequest.Type == PostTypes.SubPost)
            {
                response.Order = commentNotiRequest.Order = order;
                var post = await _postRepository.GetByIdAsync(response.PostIdOfPost);
                commentNotiRequest.PostHashId = post.HashId;
            }
            commentNotiRequest.CommentId = req.ReplyToCommentId;
            await _notificationService.AddReplyNotification(commentNotiRequest);
        }

        response.UserAvatar = userAvatar;
        response.ReplyText = await _businessText.Process(req.ReplyText);
        response.CustomNote = req.CustomNote;

        return response;
    }

    public async Task<ReplyCommentResp> UpdateReplyComment(UpdateReplyCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (!ValidReplyComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        #region -- Validate on server --
        // Commnent
        var ett = req.Type == "post"
                ? await _context.StoryPostCommentAvailable
                        .Where(p => p.Id == req.ReplyCommentId)
                        .Select(p => new { p.CreatedBy })
                        .FirstOrDefaultAsync()
                : await _context.StorySubPostCommentAvailable
                        .Where(p => p.Id == req.ReplyCommentId)
                        .Select(p => new { p.CreatedBy })
                        .FirstOrDefaultAsync();

        if (ett == null)
        {
            throw new NotFoundException(E204, M204);
        }
        if (ett.CreatedBy != userId)
        {
            throw new ForbiddenAccessException(nameof(E309), E309);
        }
        #endregion

        req.ReplyText = req.ReplyText.RemoveMaliciousText();
        var userName = req.UserName;
        var profileName = req.ProfileName;
        var userFolder = req.UserFolder;
        var userAvatar = req.UserAvatar;

        var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
        var author = new AuthorDto() { Id = userId.Value, Name = userName, Avatar = userAvatar };
        var rcDto = new ResourceCommentDto(userFolder, req.PostId, req.ResourceHashId, req.MicroService);
        var resource = await _resourceCommentService.AddResourceToComment(rcDto);
        var pDto = new PostDto();
        var response = new ReplyCommentResp();

        if (req.Type == PostTypes.Post)
        {
            var post = await _postRepository.GetByIdAsync(req.PostId);
            if (post != null)
            {
                pDto.Id = post.Id;
                pDto.HashId = post.HashId;
                pDto.CreateBy = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;
                response = await UpdateReplyToPostComment(req, author, resource, pDto);
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

                response = await UpdateReplyToSubPostComment(req, author, resource, pDto);
            }
        }

        response.AuthorName = authorName;
        response.UserName = userName;
        response.UserAvatar = userAvatar;
        response.ReplyText = await _businessText.Process(req.ReplyText);
        response.CustomNote = req.CustomNote;
        return response;
    }

    public async Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (req.ReplyCommentId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        if (req.Type == PostTypes.Post)
        {
            return await DeleteReplyToPostComment(req, userId.Value);
        }
        else
        {
            return await DeleteReplyToSubPostComment(req, userId.Value);
        }
    }

    #region Add New Rely Comment
    private async Task<ReplyCommentResp> ReplyToPostComment(ReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new StoryPostComment
        {
            AuthorId = author.Id,
            Body = req.ReplyText.RemoveMaliciousText(),
            CreatedBy = author.Id,
            ParentId = req.ReplyToCommentId,
            ModifiedBy = author.Id,
            PostId = req.PostId,
            Status = CommentStatus.Public,
            ResourceId = resource?.Id ?? null,
            GifId = req.GifId,
            QuoteId = req?.QuoteId == Guid.Empty ? null : req.QuoteId,
            CustomNote = req.CustomNote
        };

        await _postCommentRepository.InsertAsync(comment);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.ModifiedOn.Value,
            Type = PostTypes.Post,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = comment.GifId,
            Mentions = req.Mentions,
            QuoteId = comment?.QuoteId == Guid.Empty ? null : comment.QuoteId,
        };
    }

    private async Task<ReplyCommentResp> ReplyToSubPostComment(ReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new StorySubPostComment
        {
            AuthorId = author.Id,
            Body = req.ReplyText.RemoveMaliciousText(),
            CreatedBy = author.Id,
            ParentId = req.ReplyToCommentId,
            ModifiedBy = author.Id,
            PostId = req.PostId,
            Status = CommentStatus.Public,
            ResourceId = resource?.Id ?? null,
            GifId = req.GifId,
            CustomNote = req.CustomNote,
            QuoteId = req?.QuoteId == Guid.Empty ? null : req.QuoteId
        };

        await _subPostCommentRepository.InsertAsync(comment);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.ModifiedOn.Value,
            Type = PostTypes.SubPost,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = comment.GifId,
            Mentions = req.Mentions,
            QuoteId = comment?.QuoteId == Guid.Empty ? null : comment.QuoteId
        };
    }
    #endregion

    #region Update
    private async Task<ReplyCommentResp> UpdateReplyToPostComment(UpdateReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = await _postCommentRepository.GetByIdAsync(req.ReplyCommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != author.Id)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        comment.Body = req.ReplyText.RemoveMaliciousText();
        comment.ParentId = req.ReplyToCommentId;
        comment.ModifiedBy = author.Id;
        comment.ModifiedOn = DateTime.UtcNow;
        comment.ResourceId = resource?.Id ?? null;
        comment.GifId = req.GifId;
        comment.CustomNote = req.CustomNote;
        await _postCommentRepository.UpdateAsync(comment);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.ModifiedOn.Value,
            Type = PostTypes.Post,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = comment.GifId,
            Mentions = req.Mentions,
            QuoteId = comment?.QuoteId == Guid.Empty ? null : comment.QuoteId
        };
    }
    private async Task<ReplyCommentResp> UpdateReplyToSubPostComment(UpdateReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = await _subPostCommentRepository.GetByIdAsync(req.ReplyCommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != author.Id)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        comment.Body = req.ReplyText.RemoveMaliciousText();
        comment.ParentId = req.ReplyToCommentId;
        comment.ModifiedBy = author.Id;
        comment.ModifiedOn = DateTime.UtcNow;
        comment.ResourceId = resource?.Id ?? null;
        comment.GifId = req.GifId;
        comment.CustomNote = req.CustomNote;
        await _subPostCommentRepository.UpdateAsync(comment);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.ModifiedOn.Value,
            Type = PostTypes.SubPost,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value,
            ResourceHashId = resource?.HashId ?? null,
            ResourceUrl = resource?.Url ?? null,
            AuthorId = comment.AuthorId,
            GifId = comment.GifId,
            Mentions = req.Mentions,
            QuoteId = comment?.QuoteId == Guid.Empty ? null : comment.QuoteId
        };
    }
    #endregion

    #region Delete
    private async Task<ReplyCommentResp> DeleteReplyToPostComment(DeleteReplyCommentReq req, Guid userId)
    {
        var comment = await _postCommentRepository.GetByIdAsync(req.ReplyCommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != userId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var command = string.Format(DeleteReplyCommand, _postCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _postCommentRepository.Connection.ExecuteAsync(command,
                    new
                    {
                        Id = req.ReplyCommentId,
                        ModifiedBy = userId,
                        ModifiedOn = DateTime.UtcNow,
                        LocationType = (int)MentionLocationType.PostCommentReply
                    });

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyDate = comment.ModifiedOn.Value,
            Type = PostTypes.Post,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value
        };
    }
    private async Task<ReplyCommentResp> DeleteReplyToSubPostComment(DeleteReplyCommentReq req, Guid userId)
    {
        var comment = await _subPostCommentRepository.GetByIdAsync(req.ReplyCommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != userId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var command = string.Format(DeleteReplyCommand, _subPostCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _subPostCommentRepository.Connection.ExecuteAsync(command,
                        new
                        {
                            Id = req.ReplyCommentId,
                            ModifiedBy = userId,
                            ModifiedOn = DateTime.UtcNow,
                            LocationType = (int)MentionLocationType.SubPostCommentReply
                        });

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyDate = comment.ModifiedOn.Value,
            Type = PostTypes.SubPost,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value,
        };
    }
    #endregion

    private bool ValidReplyComment(ReplyCommentReq req)
    {
        if (req.PostId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidPostId, RealtimeErrorMessage.InvalidPostId);
        }
        if (req.ReplyToCommentId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidReplyToCommentId, RealtimeErrorMessage.InvalidReplyToCommentId);
        }
        if (string.IsNullOrWhiteSpace(req.ReplyText + req.ResourceHashId + req.GifId))
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
