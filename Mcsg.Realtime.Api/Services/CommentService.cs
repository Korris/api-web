using AutoMapper;
using Dapper;

namespace Mcsg.Realtime.Api.Services
{
    using Common.Core.Constants;
    using Common.Core.Enums;
    using Common.SeedWork.Exceptions;
    using Constants;
    using Dtos;
    using Interfaces;
    using Lib.Common.Constants;
    using Lib.Common.Helpers;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Requests;

    public interface ICommentService
    {
        Task<PostCommentResp> PostComment(PostCommentReq req);
        Task<PostCommentResp> UpdateComment(UpdateCommentReq req);
        Task<PostCommentResp> DeleteComment(DeleteCommentReq req);
    }

    public partial class CommentService : ICommentService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IRepository<PostComment> _postCommentRepository;
        private readonly IRepository<SubPostComment> _subPostCommentRepository;
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IRepository<Mention> _mentionRepository;
        private readonly IResourceCommentService _resourceCommentService;
        private readonly INotificationService _notificationService;
        private readonly IMentionService _mentionService;
        private readonly ISmartCountService _smartCountService;
        private readonly IMapper _mapper;
        private IConfiguration _configuration;

        public CommentService(ICurrentUserService currentUserService,
            IRepository<Post> postRepository,
            IRepository<SubPost> subPostRepository,
            IRepository<PostComment> postCommentRepository,
            IRepository<SubPostComment> subPostCommentRepository,
            IRepository<Resource> resourceRepository,
            IRepository<Mention> mentionRepository,
            IResourceCommentService resourceCommentService,
            INotificationService notificationService,
            IMentionService mentionService,
            ISmartCountService smartCountService,
            IMapper mapper,
            ISetting setting,
            IConfiguration configuration)
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
            _mapper = mapper;
            _setting = setting;
            _configuration = configuration;
        }

        public async Task<PostCommentResp> PostComment(PostCommentReq req)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
            }

            if (!ValidComment(req))
            {
                throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
            }
            var response = new PostCommentResp();

            var userName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserName)?.Value ?? "";
            var profileName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.ProfileName)?.Value ?? "";
            var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
            var userAvatar = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserAvatar)?.Value ?? "";
            var avatar = !string.IsNullOrWhiteSpace(userAvatar) ? UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, userAvatar) : "";

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

                    response = await CommentToPost(req, author, resource, pDto);
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

                    response = await CommentToSubPost(req, author, resource, pDto);
                };
            }

            if (!string.IsNullOrEmpty(pDto.HashId))
            {
                response.AuthorName = authorName;
                response.PostHashId = pDto.HashId;
                response.PostCreatedBy = pDto.CreateBy;



                // Send notification
                var commentNotiRequest = _mapper.Map<CommentNotificationReq>(response);
                await _notificationService.AddCommentNotification(commentNotiRequest);
            }

            response.UserAvatar = avatar;

            return response;
        }
        public async Task<PostCommentResp> UpdateComment(UpdateCommentReq req)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
            }

            if (!ValidComment(req))
            {
                throw new NotFoundException(RealtimeErrorCode.InvalidRequest, RealtimeErrorCode.InvalidRequest);
            }

            var userName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserName)?.Value ?? "";
            var profileName = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.ProfileName)?.Value ?? "";
            var authorName = !string.IsNullOrWhiteSpace(profileName) ? profileName : userName;
            var userAvatar = user.Claims.FirstOrDefault(x => x.Type == Setting.SecurityClaim.UserAvatar)?.Value ?? "";
            var avatar = !string.IsNullOrWhiteSpace(userAvatar) ? UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, userAvatar) : "";

            var author = new AuthorModel() { Id = user.UserId.Value, Name = userName, Avatar = avatar };

            var resource = await _resourceCommentService.AddResourceToComment(userName, req.ResourceHashId, req.Type == PostTypes.Post ? ResourceLocationType.POST_COMMENT : ResourceLocationType.SUB_POST_COMMENT);

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
            response.UserAvatar = avatar;

            return response;
        }
        public async Task<PostCommentResp> DeleteComment(DeleteCommentReq req)
        {
            var user = await _currentUserService.GetCurrentUserAsync();
            if (user == null)
            {
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
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
        private async Task<PostCommentResp> CommentToPost(PostCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
        {
            var comment = new PostComment
            {
                AuthorId = author.Id,
                Body = req.CommentText,
                CreatedBy = author.Id,
                LastModifiedBy = author.Id,
                PostId = req.PostId,
                Status = CommentStatus.PUBLIC,
                ResourceId = resource?.Id ?? null,
                GifId = req.GifId
            };

            await _postCommentRepository.InsertAsync(comment);
            //PING COUNT
            await _smartCountService.QueueAddCommentCount(req.PostId, EntityType.Post);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostComment, author, req.Mentions, post);

            return new PostCommentResp
            {
                PostId = comment.PostId,
                CommentText = comment.Body,
                CommentDate = comment.LastModifiedDate.Value,
                Type = PostTypes.Post,
                Id = comment.Id,
                ResourceHashId = resource?.HashId ?? null,
                ResourceUrl = resource?.Url ?? null,
                AuthorId = comment.AuthorId,
                GifId = req.GifId,
                Mentions = req.Mentions
            };
        }
        private async Task<PostCommentResp> CommentToSubPost(PostCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
        {
            var comment = new SubPostComment
            {
                AuthorId = author.Id,
                Body = req.CommentText,
                CreatedBy = author.Id,
                LastModifiedBy = author.Id,
                PostId = req.PostId,
                Status = CommentStatus.PUBLIC,
                ResourceId = resource?.Id ?? null,
                GifId = req.GifId
            };
            await _subPostCommentRepository.InsertAsync(comment);
            //PING COUNT
            await _smartCountService.QueueAddCommentCount(req.PostId, EntityType.SubPost);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostComment, author, req.Mentions, post);

            return new PostCommentResp
            {
                PostId = comment.PostId,
                CommentText = comment.Body,
                CommentDate = comment.LastModifiedDate.Value,
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
        private async Task<PostCommentResp> UpdateCommentToPost(UpdateCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
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
            comment.LastModifiedBy = author.Id;
            comment.LastModifiedDate = DateTime.UtcNow;
            comment.ResourceId = resource?.Id ?? null;
            comment.GifId = req.GifId;
            await _postCommentRepository.UpdateAsync(comment);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.PostComment, author, req.Mentions, post);

            return new PostCommentResp
            {
                PostId = comment.PostId,
                CommentText = comment.Body,
                CommentDate = comment.LastModifiedDate.Value,
                Type = PostTypes.Post,
                Id = comment.Id,
                ResourceHashId = resource?.HashId ?? null,
                ResourceUrl = resource?.Url ?? null,
                AuthorId = comment.AuthorId,
                GifId = comment.GifId,
                Mentions = req.Mentions
            };
        }
        private async Task<PostCommentResp> UpdateCommentToSubPost(UpdateCommentReq req, AuthorModel author, ResourceCommentResp resource, PostDto post)
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
            comment.LastModifiedBy = author.Id;
            comment.LastModifiedDate = DateTime.UtcNow;
            comment.ResourceId = resource?.Id ?? null;
            comment.GifId = req.GifId;
            await _subPostCommentRepository.UpdateAsync(comment);

            await _mentionService.AddUserMentionOnComment(comment.Id, MentionLocationType.SubPostComment, author, req.Mentions, post);

            return new PostCommentResp
            {
                PostId = comment.PostId,
                CommentText = comment.Body,
                CommentDate = comment.LastModifiedDate.Value,
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
                            LastModifiedBy = userId,
                            LastModifiedDate = DateTime.UtcNow,
                            LocationType = (int)MentionLocationType.PostComment
                        });

            //PING COUNT
            await _smartCountService.QueueRemoveCommentCount(req.CommentId, EntityType.Post);
            return new PostCommentResp
            {
                PostId = comment.PostId,
                CommentDate = comment.LastModifiedDate.Value,
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
                                LastModifiedBy = userId,
                                LastModifiedDate = DateTime.UtcNow,
                                LocationType = (int)MentionLocationType.SubPostComment
                            });

            //PING COUNT
            await _smartCountService.QueueRemoveCommentCount(req.CommentId, EntityType.SubPost);
            return new PostCommentResp
            {
                PostId = comment.PostId,
                CommentDate = comment.LastModifiedDate.Value,
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

        #endregion
    }
}
