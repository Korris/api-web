using AutoMapper;
using Dapper;

namespace Mcsg.Realtime.Api.Services
{
    using Common.Core.Constants;
    using Common.Core.Extensions;
    using Common.SeedWork.Exceptions;
    using Constants;
    using Dtos;
    using Interfaces;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Requests;
    using static Common.SeedWork.Constants.Error;
    using static Common.SeedWork.Constants.Message;

    public interface IReplyService
    {
        Task<ReplyCommentResp> ReplyComment(ReplyCommentReq req);
        Task<ReplyCommentResp> UpdateReplyComment(UpdateReplyCommentReq req);
        Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req);
    }

    public partial class ReplyService : IReplyService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IRepository<PostComment> _postCommentRepository;
        private readonly IRepository<SubPostComment> _subPostCommentRepository;
        private readonly IResourceCommentService _resourceCommentService;
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IRepository<Mention> _mentionRepository;
        private readonly INotificationService _notificationService;
        private readonly IMentionService _mentionService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;

        public ReplyService(ICurrentUserService currentUserService,
            IRepository<Post> postRepository,
            IRepository<SubPost> subPostRepository,
            IRepository<PostComment> postCommentRepository,
            IRepository<SubPostComment> subPostCommentRepository,
            IResourceCommentService resourceCommentService,
            IRepository<Resource> resourceRepository,
            IRepository<Mention> mentionRepository,
            INotificationService notificationService,
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
                throw new NotFoundException(E203, M203);
            }

            if (!ValidReplyComment(req))
            {
                throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
            }

            var userName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserName)?.Value ?? "";
            var profileName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.ProfileName)?.Value ?? "";
            var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
            var userAvatar = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserAvatar)?.Value ?? "";
            var avatar = !string.IsNullOrWhiteSpace(userAvatar) ? _setting.Minio.MediaApiUrl.ToPublicImageUrl(userAvatar) : "";

            var author = new AuthorModel() { Id = user.UserId.Value, Name = userName, Avatar = avatar };

            var resource = await _resourceCommentService.AddResourceToComment(userName, req.ResourceHashId, req.Type == PostTypes.Post ? ResourceLocationType.POST_COMMENT : ResourceLocationType.SUB_POST_COMMENT);

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
                response.UserAvatar = avatar;

                // Send notification
                var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
                await _notificationService.AddReplyNotification(commentNotiRequest);
            }

            return response;
        }
        public async Task<ReplyCommentResp> UpdateReplyComment(UpdateReplyCommentReq req)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                throw new NotFoundException(E203, M203);
            }

            if (!ValidReplyComment(req))
            {
                throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
            }

            var userName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserName)?.Value ?? "";
            var profileName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.ProfileName)?.Value ?? "";
            var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
            var userAvatar = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserAvatar)?.Value ?? "";
            var avatar = !string.IsNullOrWhiteSpace(userAvatar) ? _setting.Minio.MediaApiUrl.ToPublicImageUrl(userAvatar) : "";

            var author = new AuthorModel() { Id = user.UserId.Value, Name = userName, Avatar = avatar };
            var resource = await _resourceCommentService.AddResourceToComment(userName, req.ResourceHashId, req.Type == PostTypes.Post ? ResourceLocationType.POST_COMMENT : ResourceLocationType.SUB_POST_COMMENT);

            var response = new ReplyCommentResp();
            var pDto = new PostDto();

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
            response.UserAvatar = avatar;

            return response;
        }
        public async Task<ReplyCommentResp> DeleteReplyComment(DeleteReplyCommentReq req)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                throw new NotFoundException(E203, M203);
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
        private async Task<ReplyCommentResp> ReplyToPostComment(ReplyCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
        {
            var comment = new PostComment
            {
                AuthorId = author.Id,
                Body = req.ReplyText,
                CreatedBy = author.Id,
                ParentId = req.ReplyToCommentId,
                LastModifiedBy = author.Id,
                PostId = req.PostId,
                Status = CommentStatus.PUBLIC,
                ResourceId = resource?.Id ?? null,
                GifId = req.GifId,
                QuoteId = req?.QuoteId == Guid.Empty ? null : req.QuoteId,
            };

            await _postCommentRepository.InsertAsync(comment);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostCommentReply, author, req.Mentions, post);

            return new ReplyCommentResp
            {
                PostId = comment.PostId,
                ReplyText = comment.Body,
                ReplyDate = comment.LastModifiedDate.Value,
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

        private async Task<ReplyCommentResp> ReplyToSubPostComment(ReplyCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
        {
            var comment = new SubPostComment
            {
                AuthorId = author.Id,
                Body = req.ReplyText,
                CreatedBy = author.Id,
                ParentId = req.ReplyToCommentId,
                LastModifiedBy = author.Id,
                PostId = req.PostId,
                Status = CommentStatus.PUBLIC,
                ResourceId = resource?.Id ?? null,
                GifId = req.GifId,
            };

            await _subPostCommentRepository.InsertAsync(comment);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostCommentReply, author, req.Mentions, post);

            return new ReplyCommentResp
            {
                PostId = comment.PostId,
                ReplyText = comment.Body,
                ReplyDate = comment.LastModifiedDate.Value,
                Type = PostTypes.SubPost,
                Id = comment.Id,
                ReplyToCommentId = comment.ParentId.Value,
                ResourceHashId = resource?.HashId ?? null,
                ResourceUrl = resource?.Url ?? null,
                AuthorId = comment.AuthorId,
                GifId = comment.GifId,
                Mentions = req.Mentions
            };
        }
        #endregion

        #region Update
        private async Task<ReplyCommentResp> UpdateReplyToPostComment(UpdateReplyCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
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
            comment.LastModifiedBy = author.Id;
            comment.LastModifiedDate = DateTime.UtcNow;
            comment.ResourceId = resource?.Id ?? null;
            comment.GifId = req.GifId;
            await _postCommentRepository.UpdateAsync(comment);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostCommentReply, author, req.Mentions, post);

            return new ReplyCommentResp
            {
                PostId = comment.PostId,
                ReplyText = comment.Body,
                ReplyDate = comment.LastModifiedDate.Value,
                Type = PostTypes.Post,
                Id = comment.Id,
                ReplyToCommentId = comment.ParentId.Value,
                ResourceHashId = resource?.HashId ?? null,
                ResourceUrl = resource?.Url ?? null,
                AuthorId = comment.AuthorId,
                GifId = comment.GifId,
                Mentions = req.Mentions
            };
        }
        private async Task<ReplyCommentResp> UpdateReplyToSubPostComment(UpdateReplyCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
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
            comment.LastModifiedBy = author.Id;
            comment.LastModifiedDate = DateTime.UtcNow;
            comment.ResourceId = resource?.Id ?? null;
            comment.GifId = req.GifId;
            await _subPostCommentRepository.UpdateAsync(comment);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostCommentReply, author, req.Mentions, post);

            return new ReplyCommentResp
            {
                PostId = comment.PostId,
                ReplyText = comment.Body,
                ReplyDate = comment.LastModifiedDate.Value,
                Type = PostTypes.SubPost,
                Id = comment.Id,
                ReplyToCommentId = comment.ParentId.Value,
                ResourceHashId = resource?.HashId ?? null,
                ResourceUrl = resource?.Url ?? null,
                AuthorId = comment.AuthorId,
                GifId = comment.GifId,
                Mentions = req.Mentions
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
                            LastModifiedBy = userId,
                            LastModifiedDate = DateTime.UtcNow,
                            LocationType = (int)MentionLocationType.PostCommentReply
                        });

            return new ReplyCommentResp
            {
                PostId = comment.PostId,
                ReplyDate = comment.LastModifiedDate.Value,
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
                                LastModifiedBy = userId,
                                LastModifiedDate = DateTime.UtcNow,
                                LocationType = (int)MentionLocationType.SubPostCommentReply
                            });

            return new ReplyCommentResp
            {
                PostId = comment.PostId,
                ReplyDate = comment.LastModifiedDate.Value,
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
}
