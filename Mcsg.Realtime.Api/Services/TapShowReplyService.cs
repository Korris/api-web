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
using Constants;
using Dtos;
using Interfaces;
using Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// TapShow reply service (copy of StoryReplyService, post comments only: TapShow has no chapter comments)
/// </summary>
public partial class TapShowReplyService : BaseS, ITapShowReplyService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="businessText"></param>
    /// <param name="postCommentRepository"></param>
    /// <param name="resourceCommentService"></param>
    /// <param name="resourceRepository"></param>
    /// <param name="mentionRepository"></param>
    /// <param name="notificationService"></param>
    /// <param name="mentionService"></param>
    /// <param name="mapper"></param>
    public TapShowReplyService(IMcsgContext context, IBusinessText businessText, IRepository<TapShowPostComment> postCommentRepository, IResourceCommentService resourceCommentService, IRepository<TapShowResource> resourceRepository, IRepository<Mention> mentionRepository, INotificationService notificationService, IMentionService mentionService, IMapper mapper) : base(context)
    {
        _businessText = businessText;

        _postCommentRepository = postCommentRepository;
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

        var post = await _context.Available<TapShowPost>().FirstOrDefaultAsync(p => p.Id == req.PostId);
        if (post != null)
        {
            pDto.Id = post.Id;
            pDto.HashId = post.HashId;
            pDto.CreateBy = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;

            response = await ReplyToPostComment(req, author, resource, pDto);
        }

        if (!string.IsNullOrWhiteSpace(pDto.HashId))
        {
            response.AuthorName = authorName;
            response.UserAvatar = userAvatar;
            response.UserName = userName;
            response.QuoteId = req.QuoteId;
            // Send notification
            response.PostType = PostType.TapShow;
            var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
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
        var ett = await _context.Available<TapShowPostComment>()
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

        var post = await _context.Available<TapShowPost>().FirstOrDefaultAsync(p => p.Id == req.PostId);
        if (post != null)
        {
            pDto.Id = post.Id;
            pDto.HashId = post.HashId;
            pDto.CreateBy = post.CreatedBy != null ? post.CreatedBy.Value : Guid.Empty;
            response = await UpdateReplyToPostComment(req, author, resource, pDto);
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

        if (req.ReplyCommentId == Guid.Empty || req.Type != PostTypes.Post)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        return await DeleteReplyToPostComment(req);
    }

    #region Add New Rely Comment
    private async Task<ReplyCommentResp> ReplyToPostComment(ReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new TapShowPostComment
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
        await _context.TapShowPostComments.AddAsync(comment);
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
    #endregion

    #region Update
    private async Task<ReplyCommentResp> UpdateReplyToPostComment(UpdateReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = await _context.Available<TapShowPostComment>().FirstOrDefaultAsync(p => p.Id == req.ReplyCommentId);
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
    #endregion

    #region Delete
    private async Task<ReplyCommentResp> DeleteReplyToPostComment(DeleteReplyCommentReq req)
    {
        var comment = await _context.Available<TapShowPostComment>().FirstOrDefaultAsync(p => p.Id == req.ReplyCommentId);
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
    #endregion

    private bool ValidReplyComment(ReplyCommentReq req)
    {
        // TapShow only has post comments (no chapter / subpost comments)
        if (req.Type != PostTypes.Post)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }
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

    private readonly IRepository<TapShowPostComment> _postCommentRepository;
    private readonly IResourceCommentService _resourceCommentService;
    private readonly IRepository<TapShowResource> _resourceRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly INotificationService _notificationService;
    private readonly IMentionService _mentionService;
    private readonly IMapper _mapper;

    #endregion
}
