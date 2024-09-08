using AutoMapper;
using Dapper;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Constants;
using Dtos;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class StoryReplyService : IStoryReplyService
{
    private readonly ICurrentUserService _currentUserService;
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

    public StoryReplyService(ICurrentUserService currentUserService,
        IRepository<StoryPost> postRepository,
        IRepository<StorySubPost> subPostRepository,
        IRepository<StoryPostComment> postCommentRepository,
        IRepository<StorySubPostComment> subPostCommentRepository,
        IResourceCommentService resourceCommentService,
        IRepository<StoryResource> resourceRepository,
        IRepository<Mention> mentionRepository,
        IStoryNotificationService notificationService,
        IMentionService mentionService,
        IMapper mapper,
        ISetting setting,
        IConfiguration configuration)
    {
        _currentUserService = currentUserService;
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
    }

    public async Task<ReplyCommentResp> ReplyComment(ReplyCommentReq req)
    {
        var response = new ReplyCommentResp();
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null || string.IsNullOrWhiteSpace(user.SessionId))
        {
            throw new NotFoundException(E303, M303);
        }

        if (!ValidReplyComment(req))
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
                pDto.Id = subPost.Id;
                pDto.HashId = subPost.HashId;
                pDto.CreateBy = subPost.CreatedBy != null ? subPost.CreatedBy.Value : Guid.Empty;

                response = await ReplyToSubPostComment(req, author, resource, pDto);
            };
        }

        if (!string.IsNullOrWhiteSpace(pDto.HashId))
        {
            response.AuthorName = authorName;
            response.UserAvatar = userAvatar;

            // Send notification
            response.PostType = PostType.Story;
            var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
            await _notificationService.AddReplyNotification(commentNotiRequest);
        }
        
        response.CustomNote = req.CustomNote;

        return response;
    }
    public async Task<ReplyCommentResp> UpdateReplyComment(UpdateReplyCommentReq req)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (!ValidReplyComment(req))
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
        response.UserAvatar = userAvatar;
        response.CustomNote = req.CustomNote;

        return response;
    }
    public async Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req)
    {
        var user = await _currentUserService.GetCurrentUserAsync();
        if (user == null)
        {
            throw new NotFoundException(E303, M303);
        }

        if (req.ReplyCommentId == Guid.Empty)
        {
            throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
        }

        if (req.Type == PostTypes.Post)
        {
            return await DeleteReplyToPostComment(req, user.UserId.Value);
        }
        else
        {
            return await DeleteReplyToSubPostComment(req, user.UserId.Value);
        }
    }

    #region Add New Rely Comment
    private async Task<ReplyCommentResp> ReplyToPostComment(ReplyCommentReq req, AuthorDto author, ResourceCommentResp resource, PostDto post)
    {
        var comment = new StoryPostComment
        {
            AuthorId = author.Id,
            Body = req.ReplyText,
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
            Body = req.ReplyText,
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

        comment.Body = req.ReplyText;
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

        comment.Body = req.ReplyText;
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

    #endregion
}
