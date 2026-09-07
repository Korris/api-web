using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Realtime.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Mcsg.Api.Areas.Realtime.Constants;
using Mcsg.Api.Areas.Realtime.Dtos;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;
using static Common.SeedWork.Constants.Error;

public partial class StoryReplyService : BaseS, IStoryReplyService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="businessText"></param>
    /// <param name="postCommentRepository"></param>
    /// <param name="subPostCommentRepository"></param>
    /// <param name="resourceCommentService"></param>
    /// <param name="resourceRepository"></param>
    /// <param name="mentionRepository"></param>
    /// <param name="notificationService"></param>
    /// <param name="mentionService"></param>
    /// <param name="mapper"></param>
    public StoryReplyService(IMcsgContext context, IBusinessText businessText, IRepository<StoryPostComment> postCommentRepository, IRepository<StorySubPostComment> subPostCommentRepository, IResourceCommentService resourceCommentService, IRepository<StoryResource> resourceRepository, IRepository<Mention> mentionRepository, INotificationService notificationService, IMentionService mentionService, IMapper mapper) : base(context)
    {
        _businessText = businessText;

        _postCommentRepository = postCommentRepository;
        _subPostCommentRepository = subPostCommentRepository;
        _resourceCommentService = resourceCommentService;
        _resourceRepository = resourceRepository;
        _mentionRepository = mentionRepository;
        _notificationService = notificationService;
        _mentionService = mentionService;
        _mapper = mapper;
    }

    public async Task<ReplyCommentResp> ReplyComment(ReplyCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        var response = new ReplyCommentResp();

        if (!ValidReplyComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        req.ReplyText = req.ReplyText.RemoveMaliciousText();
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
        var order = 0.0f;

        if (req.Type == PostTypes.Post)
        {
            var post = await _context.Available<StoryPost>().FirstOrDefaultAsync(p => p.Id == req.PostId);
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
            var subPost = await _context.Available<StorySubPost>().FirstOrDefaultAsync(p => p.Id == req.PostId);
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
                var post = await _context.Available<StoryPost>().FirstOrDefaultAsync(p => p.Id == response.PostIdOfPost);
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
            throw new NotFoundException(nameof(E303), E303);
        }

        if (!ValidReplyComment(req))
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        #region -- Validate on server --
        // Commnent
        var ett = req.Type == "post"
                ? await _context.Available<StoryPostComment>()
                        .Where(p => p.Id == req.ReplyCommentId)
                        .Select(p => new { p.CreatedBy })
                        .FirstOrDefaultAsync()
                : await _context.Available<StorySubPostComment>()
                        .Where(p => p.Id == req.ReplyCommentId)
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

        req.ReplyText = req.ReplyText.RemoveMaliciousText();
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
        var response = new ReplyCommentResp();

        if (req.Type == PostTypes.Post)
        {
            var post = await _context.Available<StoryPost>().FirstOrDefaultAsync(p => p.Id == req.PostId);
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
            var subPost = await _context.Available<StorySubPost>().FirstOrDefaultAsync(p => p.Id == req.PostId);
            if (subPost != null)
            {
                pDto.Id = subPost.Id;
                pDto.HashId = subPost.HashId;
                pDto.CreateBy = subPost.CreatedBy != null ? subPost.CreatedBy.Value : Guid.Empty;

                response = await UpdateReplyToSubPostComment(req, author, resource, pDto);
            }
        }

        response.AuthorName = authorName;
        response.UserAvatar = userAvatar;
        response.UserName = userName;
        response.ReplyText = await _businessText.Process(req.ReplyText);
        response.CustomNote = req.CustomNote;

        return response;
    }

    public async Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req)
    {
        var userId = req.UserId;
        if (userId == null)
        {
            throw new NotFoundException(nameof(E303), E303);
        }

        if (req.ReplyCommentId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        if (req.Type == PostTypes.Post)
        {
            return await DeleteReplyToPostComment(req);
        }
        else
        {
            return await DeleteReplyToSubPostComment(req);
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
            CustomNote = req.CustomNote.RemoveMaliciousText()
        };
        await _context.StoryPostComments.AddAsync(comment);
        await _context.SaveChangesAsync(default);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.CreatedOn,
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
            CustomNote = req.CustomNote.RemoveMaliciousText(),
            QuoteId = req?.QuoteId == Guid.Empty ? null : req.QuoteId,
        };
        await _context.StorySubPostComments.AddAsync(comment);
        await _context.SaveChangesAsync(default);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.CreatedOn,
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
        var comment = await _context.Available<StoryPostComment>().FirstOrDefaultAsync(p => p.Id == req.ReplyCommentId);
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
        comment.CustomNote = req.CustomNote.RemoveMaliciousText();
        await _context.SaveChangesAsync(default);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.CreatedOn,
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
        var comment = await _context.Available<StorySubPostComment>().FirstOrDefaultAsync(p => p.Id == req.ReplyCommentId);
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
        comment.CustomNote = req.CustomNote.RemoveMaliciousText();
        await _context.SaveChangesAsync(default);

        await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostCommentReply, author, req.Mentions, post);

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyText = comment.Body,
            ReplyDate = comment.CreatedOn,
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
    private async Task<ReplyCommentResp> DeleteReplyToPostComment(DeleteReplyCommentReq req)
    {
        var comment = await _context.Available<StoryPostComment>().FirstOrDefaultAsync(p => p.Id == req.ReplyCommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != req.UserId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var command = string.Format(DeleteReplyCommand, _postCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _postCommentRepository.Connection.ExecuteAsync(command,
                    new
                    {
                        Id = req.ReplyCommentId,
                        ModifiedBy = req.UserId,
                        ModifiedOn = DateTime.UtcNow,
                        LocationType = (int)MentionLocationType.PostCommentReply
                    });

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyDate = comment.CreatedOn,
            Type = PostTypes.Post,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value
        };
    }
    private async Task<ReplyCommentResp> DeleteReplyToSubPostComment(DeleteReplyCommentReq req)
    {
        var comment = await _context.Available<StorySubPostComment>().FirstOrDefaultAsync(p => p.Id == req.ReplyCommentId);
        if (comment == null)
        {
            throw new NotFoundException(RealtimeErrorCode.NotFoundComment, RealtimeErrorMessage.NotFoundComment);
        }

        if (comment.AuthorId != req.UserId)
        {
            throw new NotFoundException(RealtimeErrorCode.UnAuthorizeUpdate, RealtimeErrorMessage.UnAuthorizeUpdate);
        }

        var postIdOfPost = await _context.StorySubPosts
            .Where(sp => sp.Id == comment.PostId)
            .Select(sp => sp.PostId)
            .FirstOrDefaultAsync();

        var command = string.Format(DeleteReplyCommand, _subPostCommentRepository.TableName, _resourceRepository.TableName, _mentionRepository.TableName);
        await _subPostCommentRepository.Connection.ExecuteAsync(command,
                        new
                        {
                            Id = req.ReplyCommentId,
                            ModifiedBy = req.UserId,
                            ModifiedOn = DateTime.UtcNow,
                            LocationType = (int)MentionLocationType.SubPostCommentReply
                        });

        return new ReplyCommentResp
        {
            PostId = comment.PostId,
            ReplyDate = comment.CreatedOn,
            Type = PostTypes.SubPost,
            Id = comment.Id,
            ReplyToCommentId = comment.ParentId.Value,
            PostIdOfPost = postIdOfPost
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
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    private readonly IRepository<StoryPostComment> _postCommentRepository;
    private readonly IRepository<StorySubPostComment> _subPostCommentRepository;
    private readonly IResourceCommentService _resourceCommentService;
    private readonly IRepository<StoryResource> _resourceRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly INotificationService _notificationService;
    private readonly IMentionService _mentionService;
    private readonly IMapper _mapper;

    #endregion
}
