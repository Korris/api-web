using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;

namespace Mcsg.Social.Api.Services
{
    using Api.Interfaces;
    using Common.Core.Extensions;
    using DTOs;
    using Enums;
    using Extensions;
    using Lib.Common.Helpers;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Entities.Common;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Lib.Model.Enums;
    using Models;

    public interface ICommentService
    {
        Task<PagedResults<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
        Task<PagedResults<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadReq request);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadReq request, PostType postType);
        Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(MostReactionCommentInput input);
        Task<List<BasicCommentResponse>> GetReplyByCommentId(ReplyByCommentInput input);
    }
    public partial class CommentService : ICommentService
    {
        private readonly IRepository<PostComment> _postCommentRepository;
        private readonly IRepository<SubPostComment> _subPostCommentRepository;
        private readonly IRepository<SubPost> _subPostRepository;
        private readonly IRepository<Resource> _resourceRepository;
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Mention> _mentionRepository;
        private readonly FileSetting _fileSetting;
        private IConfiguration _configuration;
        protected readonly IMapper _mapper;
        public CommentService(IUnitOfWork unitOfWork, IOptionsMonitor<FileSetting> fileSetting, IMapper mapper, ISetting setting, IConfiguration configuration)
        {
            _postCommentRepository = unitOfWork.GetRepository<PostComment>();
            _subPostCommentRepository = unitOfWork.GetRepository<SubPostComment>();
            _resourceRepository = unitOfWork.GetRepository<Resource>();
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _userRepository = unitOfWork.GetRepository<User>();
            _mentionRepository = unitOfWork.GetRepository<Mention>();
            _fileSetting = fileSetting.CurrentValue;
            _mapper = mapper;
            _setting = setting;
            _configuration = configuration;
        }

        public async Task<PagedResults<CommentResponse>> GetLatestPostCommentInAsync(Guid postId)
        {
            PagedResults<CommentResponse> response;

            var result = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<CommentQueryResult>(GetCommentByPostInHomePageQuery, new { PostId = postId });

            if (result != null)
            {
                response = new PagedResults<CommentResponse>(1, 1, 1);
                var commentData = new CommentResponse()
                {
                    Id = result.Id,
                    AuthorId = result.AuthorId,
                    AuthorName = result.AuthorName,
                    UserAvatar = result.UserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                    Body = result.Body,
                    PostId = result.PostId,
                    LastModifiedDate = result.LastModifiedDate,
                    ResourceHashId = result.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, result.ResourceName, result.ResourceUrl) : "",
                    GifId = result.GifId
                };

                var replies = new ReplyResponse()
                {
                    TotalReply = (result.ReplyId != Guid.Empty) ? 1 : 0,
                    Data = new List<ReplyData>()
                };

                if (result.ReplyId != Guid.Empty)
                {
                    var replyData = new ReplyData()
                    {
                        Id = result.ReplyId,
                        AuthorId = result.AuthorId,
                        AuthorName = result.ReplyAuthorName,
                        UserAvatar = result.ReplyUserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                        Body = result.ReplyBody,
                        LastModifiedDate = result.ReplyLastModifiedDate,
                        ResourceHashId = result.ReplyResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, result.ReplyResourceName, result.ReplyResourceUrl) : "",
                        ParentId = commentData.Id,
                        GifId = result.ReplyGifId,
                        QuoteId = result?.ReplyQuoteId == Guid.Empty ? null : result.ReplyQuoteId
                    };

                    replies.Data.Add(replyData);
                }

                commentData.Replies = replies;

                // Mention 
                var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments,
                                        new
                                        {
                                            LocationIds = new List<Guid> { commentData.Id }
                                        });
                commentData.Mentions = _mapper.Map<List<UserMentionResponse>>(mentions);

                response.Items = new List<CommentResponse>() { commentData };
                response.TotalItems = result.TotalRecord;
            }
            else
            {
                response = new PagedResults<CommentResponse>(0);
            }
            return response;
        }
        public async Task<PagedResults<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId)
        {
            PagedResults<CommentResponse> response;

            var result = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<CommentQueryResult>(GetCommentBySubPostQuery, new { PostId = postId });

            if (result != null)
            {
                response = new PagedResults<CommentResponse>(1, 1, 1);
                var commentData = new CommentResponse()
                {
                    Id = result.Id,
                    AuthorId = result.AuthorId,
                    AuthorName = result.AuthorName,
                    UserAvatar = result.UserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                    Body = result.Body,
                    PostId = result.PostId,
                    LastModifiedDate = result.LastModifiedDate,
                    ResourceHashId = result.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, result.ResourceName, result.ResourceUrl) : "",
                    GifId = result.GifId
                };

                var replies = new ReplyResponse()
                {
                    TotalReply = (result.ReplyId != Guid.Empty) ? 1 : 0,
                    Data = new List<ReplyData>()
                };

                if (result.ReplyId != Guid.Empty)
                {
                    var replyData = new ReplyData()
                    {
                        Id = result.ReplyId,
                        AuthorId = result.AuthorId,
                        AuthorName = result.ReplyAuthorName,
                        UserAvatar = result.ReplyUserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                        Body = result.ReplyBody,
                        LastModifiedDate = result.ReplyLastModifiedDate,
                        ResourceHashId = result.ReplyResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, result.ReplyResourceName, result.ReplyResourceUrl) : "",
                        ParentId = commentData.Id,
                        GifId = result.ReplyGifId
                    };

                    replies.Data.Add(replyData);
                }

                commentData.Replies = replies;

                // Mention 
                var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments,
                                                                                new
                                                                                {
                                                                                    LocationIds = new List<Guid> { commentData.Id }
                                                                                });
                commentData.Mentions = _mapper.Map<List<UserMentionResponse>>(mentions);

                response.Items = new List<CommentResponse>() { commentData };
                response.TotalItems = result.TotalRecord;
            }
            else
            {
                response = new PagedResults<CommentResponse>(0);
            }
            return response;
        }

        public async Task<List<BasicCommentResponse>> GetReplyByCommentId(ReplyByCommentInput input)
        {
            List<BasicCommentResponse> results;

            var query = string.Format(GetReplyByCommentIdQuery, input.IsSubPost ? _subPostCommentRepository.TableName : _postCommentRepository.TableName);
            var items = await _postCommentRepository.Connection.QueryAsync<BasicCommentResponse>(query, new { CommentId = input.CommentId });
            if (items != null && items.Count() > 0)
            {
                var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });
                foreach (var item in items)
                {
                    item.UserAvatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.UserAvatar);
                    item.ResourceUrl = !string.IsNullOrWhiteSpace(item.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, item.ResourceName, item.ResourceUrl) : "";
                    if (mentions != null && mentions.Any())
                    {
                        var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                        item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                    }
                }
                results = items.ToList();
                return results;
            }
            else
            {
                return null;
            }
        }

        public async Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(MostReactionCommentInput input)
        {
            var methodName = $"{typeof(CommentService).FullName}.{nameof(GetCommentWithMostReaction)}";
            $"{methodName} ->Begin".LogInfor();

            if (string.IsNullOrWhiteSpace(input.HashPostId))
            {
                return new CommentPagedResults<MostReactionCommentResponse>(0);
            }
            CommentPagedResults<MostReactionCommentResponse> results;
            var offset = input.PageSize * (input.PageNumber - 1);
            var query = GetCommentWithMostReactionQuery;

            var multi = await _postCommentRepository
               .Connection.QueryMultipleAsync(query, new
               {
                   HashId = input.HashPostId,
                   PageSize = input.PageSize,
                   Offset = offset
               });
            var items = await multi.ReadAsync<MostReactionCommentResponse>().ConfigureAwait(false);
            $"{methodName} ->items".LogInfor();
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });

                $"{methodName} ->foreach".LogInfor();
                foreach (var item in items)
                {
                    item.UserAvatar = UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.UserAvatar);
                    item.ResourceUrl = !string.IsNullOrWhiteSpace(item.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, item.ResourceName, item.ResourceUrl) : "";

                    if (mentions != null && mentions.Any())
                    {
                        var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                        item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                    }
                }
                $"{methodName} -|foreach".LogInfor();

                results = new CommentPagedResults<MostReactionCommentResponse>(totalItems, input.PageNumber, input.PageSize);
                results.Items = items;
                $"{methodName} ->results".LogInfor();
                results.TotalComments = await _postCommentRepository.Connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = input.HashPostId });
                $"{methodName} ->results.TotalComments={results.TotalComments}".LogInfor();
            }
            else
            {
                results = new CommentPagedResults<MostReactionCommentResponse>(0);
            }

            $"{methodName} -|End".LogInfor();

            return results;
        }

        public async Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadReq request)
        {
            if (string.IsNullOrWhiteSpace(request.OrderBy))
            {
                request.OrderBy = "LastModifiedDate";
            }
            var offset = request.PageSize * (request.PageNumber - 1);

            var query = string.Format(GetCommentOfPostQuery, request.OrderBy);
            var result = await _postCommentRepository.Connection.QueryAsync<CommentQueryModel>(query, new { PostId = request.PostId });
            var totalRecord = result?.Count() ?? 0;
            var totalComments = 0;
            var comments = new List<CommentResponse>();
            if (result != null && result.Any())
            {
                var commentModels = result.Where(x => x.CommentLevel == (int)CommentLevel.Comment).ToList();
                totalComments = commentModels.Count();
                var pagedComments = commentModels.Skip(offset).Take(request.PageSize).ToList();

                var commentIds = pagedComments.Select(x => x.Id).ToList();

                var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = commentIds });
                foreach (var comModel in pagedComments)
                {
                    var comment = new CommentResponse()
                    {
                        Id = comModel.Id,
                        PostId = comModel.PostId,
                        AuthorId = comModel.AuthorId,
                        AuthorName = comModel.AuthorName,
                        UserAvatar = comModel.UserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                        Body = comModel.Body,
                        LastModifiedDate = comModel.LastModifiedDate,
                        ResourceHashId = comModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, comModel.ResourceName, comModel.ResourceUrl) : "",
                        GifId = comModel.GifId
                    };

                    var replyModels = result.Where(x => x.ParentId == comModel.Id
                                        && x.CommentLevel == (int)CommentLevel.Reply).ToList();

                    var replies = new ReplyResponse()
                    {
                        TotalReply = replyModels.Count
                    };
                    foreach (var repModel in replyModels)
                    {
                        var replyData = new ReplyData()
                        {
                            Id = repModel.Id,
                            AuthorId = repModel.AuthorId,
                            AuthorName = repModel.AuthorName,
                            UserAvatar = repModel.UserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                            Body = repModel.Body,
                            LastModifiedDate = repModel.LastModifiedDate,
                            ResourceHashId = repModel.ResourceHashId,
                            ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, repModel.ResourceName, repModel.ResourceUrl) : "",
                            ParentId = comment.Id,
                            GifId = repModel.GifId,
                            QuoteId = repModel.QuoteId
                        };
                        replies.Data.Add(replyData);
                    }
                    comment.Replies = replies;

                    // Mention to comment
                    if (mentions != null && mentions.Any())
                    {
                        var userMentioneds = mentions.Where(x => x.LocationId == comment.Id).ToList();
                        comment.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                    }

                    comments.Add(comment);
                }
            }

            CommentPagedResults<CommentResponse> response = new CommentPagedResults<CommentResponse>(totalRecord, request.PageNumber, request.PageSize);
            response.Items = comments;
            response.TotalComments = totalComments;
            return response;
        }

        public async Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadReq request, PostType postType)
        {
            var query = GetCommentOfSubPostQuery;
            var postId = request.PostId;

            if (postType == PostType.FEED)
            {
                var totalSubPost = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<SubPostCountModel>(CountSubPostOfPostQuery, new { PostId = request.PostId });

                if (string.IsNullOrWhiteSpace(request.OrderBy))
                {
                    request.OrderBy = "LastModifiedDate";
                }

                if (totalSubPost != null && totalSubPost.Count < 2)
                {
                    // Load post comment instead of subpost comment when have post have only 1 subpost
                    query = GetCommentOfPostQuery;
                    postId = totalSubPost.PostId;
                }
            }

            query = string.Format(query, request.OrderBy);

            var offset = request.PageSize * (request.PageNumber - 1);
            var result = await _postCommentRepository.Connection.QueryAsync<CommentQueryModel>(query, new { PostId = postId });
            var totalRecord = result?.Count() ?? 0;
            var totalComments = 0;
            var comments = new List<CommentResponse>();
            if (result != null && result.Any())
            {
                var commentModels = result.Where(x => x.CommentLevel == (int)CommentLevel.Comment).ToList();
                totalComments = commentModels.Count();
                var pagedComments = commentModels.Skip(offset).Take(request.PageSize).ToList();

                var commentIds = pagedComments.Select(x => x.Id).ToList();
                var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = commentIds });
                foreach (var comModel in pagedComments)
                {
                    var comment = new CommentResponse()
                    {
                        Id = comModel.Id,
                        PostId = comModel.PostId,
                        AuthorId = comModel.AuthorId,
                        AuthorName = comModel.AuthorName,
                        UserAvatar = comModel.UserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                        Body = comModel.Body,
                        LastModifiedDate = comModel.LastModifiedDate,
                        ResourceHashId = comModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, comModel.ResourceName, comModel.ResourceUrl) : "",
                        GifId = comModel.GifId
                    };

                    var replyModels = result.Where(x => x.ParentId == comModel.Id
                                        && x.CommentLevel == (int)CommentLevel.Reply).ToList();

                    var replies = new ReplyResponse()
                    {
                        TotalReply = replyModels.Count
                    };
                    foreach (var repModel in replyModels)
                    {
                        var replyData = new ReplyData()
                        {
                            Id = repModel.Id,
                            AuthorId = repModel.AuthorId,
                            AuthorName = repModel.AuthorName,
                            UserAvatar = repModel.UserAvatar.ToPublicImageUrl(_setting.Minio.MediaApiUrl),
                            Body = repModel.Body,
                            LastModifiedDate = repModel.LastModifiedDate,
                            ResourceHashId = repModel.ResourceHashId,
                            ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, repModel.ResourceName, repModel.ResourceUrl) : "",
                            ParentId = comment.Id,
                            GifId = repModel.GifId
                        };
                        replies.Data.Add(replyData);
                    }
                    comment.Replies = replies;

                    // Mention to comment
                    if (mentions != null && mentions.Any())
                    {
                        var userMentioneds = mentions.Where(x => x.LocationId == comment.Id).ToList();
                        comment.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                    }

                    comments.Add(comment);
                }
            }

            CommentPagedResults<CommentResponse> response = new CommentPagedResults<CommentResponse>(totalRecord, request.PageNumber, request.PageSize);
            response.Items = comments;
            response.TotalComments = totalComments;
            return response;
        }

        #region -- Fields --

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        #endregion
    }
}
