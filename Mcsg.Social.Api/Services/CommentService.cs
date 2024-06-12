using AutoMapper;
using Dapper;
using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Enums;
using Mcsg.Social.Api.Extensions;
using Mcsg.Social.Api.Models;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Mcsg.Lib.Model.Enums;
using Microsoft.Extensions.Options;

namespace Mcsg.Social.Api.Services
{
    public interface ICommentService
    {
        Task<PagedResults<CommentResponse>> GetLatestPostCommentInAsync(Guid postId);
        Task<PagedResults<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadReq request);
        Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadReq request, PostType postType);
        Task<MostReactionCommentResponse> GetCommentWithMostReaction(string postHashId);
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
        public CommentService(IUnitOfWork unitOfWork, IOptionsMonitor<FileSetting> fileSetting, IMapper mapper, IConfiguration configuration)
        {
            _postCommentRepository = unitOfWork.GetRepository<PostComment>();
            _subPostCommentRepository = unitOfWork.GetRepository<SubPostComment>();
            _resourceRepository = unitOfWork.GetRepository<Resource>();
            _subPostRepository = unitOfWork.GetRepository<SubPost>();
            _userRepository = unitOfWork.GetRepository<User>();
            _mentionRepository = unitOfWork.GetRepository<Mention>();
            _fileSetting = fileSetting.CurrentValue;
            _mapper = mapper;
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
                    UserAvatar = result.UserAvatar.ToPublicImageUrl(),
                    Body = result.Body,
                    PostId = result.PostId,
                    LastModifiedDate = result.LastModifiedDate,
                    ResourceHashId = result.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, result.ResourceName, result.ResourceUrl) : "",
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
                        UserAvatar = result.ReplyUserAvatar.ToPublicImageUrl(),
                        Body = result.ReplyBody,
                        LastModifiedDate = result.ReplyLastModifiedDate,
                        ResourceHashId = result.ReplyResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, result.ReplyResourceName, result.ReplyResourceUrl) : "",
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
                    UserAvatar = result.UserAvatar.ToPublicImageUrl(),
                    Body = result.Body,
                    PostId = result.PostId,
                    LastModifiedDate = result.LastModifiedDate,
                    ResourceHashId = result.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, result.ResourceName, result.ResourceUrl) : "",
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
                        UserAvatar = result.ReplyUserAvatar.ToPublicImageUrl(),
                        Body = result.ReplyBody,
                        LastModifiedDate = result.ReplyLastModifiedDate,
                        ResourceHashId = result.ReplyResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, result.ReplyResourceName, result.ReplyResourceUrl) : "",
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
                        UserAvatar = comModel.UserAvatar.ToPublicImageUrl(),
                        Body = comModel.Body,
                        LastModifiedDate = comModel.LastModifiedDate,
                        ResourceHashId = comModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, comModel.ResourceName, comModel.ResourceUrl) : "",
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
                            UserAvatar = repModel.UserAvatar.ToPublicImageUrl(),
                            Body = repModel.Body,
                            LastModifiedDate = repModel.LastModifiedDate,
                            ResourceHashId = repModel.ResourceHashId,
                            ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, repModel.ResourceName, repModel.ResourceUrl) : "",
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
        public async Task<MostReactionCommentResponse> GetCommentWithMostReaction(string postHashId)
        {
            if (string.IsNullOrWhiteSpace(postHashId))
            {
                return new MostReactionCommentResponse();
            }

            var query = $@"SELECT 
                                    pc.""Id"", 
                                    pc.""Body"",
                                    u.""Avatar"",
                                    u.""ProfileId"",
                                    u.""ProfileName"" ,
                                    COUNT(pcr.""Id"") AS max_reaction_count
                            FROM ""Posts"" p 
                            LEFT JOIN ""Users"" u ON p.""CreatedBy""= u.""Id""
                            LEFT JOIN ""PostComments"" pc ON p.""Id""= pc.""PostId""
                            LEFT JOIN ""PostCommentReactions"" pcr ON pc.""Id""= pcr.""TargetId""
                            WHERE p.""HashId"" =@HashId
                            GROUP BY pc.""Id"" ,u.""Avatar"",u.""ProfileName"" ,u.""ProfileId"" 
                            ORDER BY max_reaction_count DESC, pc.""CreatedDate"" desc
                            LIMIT 1";
            var result = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<MostReactionCommentResponse>(query, new { HashId = postHashId });
            result.Avatar = UrlHelper.GetPublicImageUrl(_configuration, result.Avatar);
            return result;
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
                        UserAvatar = comModel.UserAvatar.ToPublicImageUrl(),
                        Body = comModel.Body,
                        LastModifiedDate = comModel.LastModifiedDate,
                        ResourceHashId = comModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, comModel.ResourceName, comModel.ResourceUrl) : "",
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
                            UserAvatar = repModel.UserAvatar.ToPublicImageUrl(),
                            Body = repModel.Body,
                            LastModifiedDate = repModel.LastModifiedDate,
                            ResourceHashId = repModel.ResourceHashId,
                            ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? UrlHelper.GetMediaPath(_fileSetting.MediaUrl, repModel.ResourceName, repModel.ResourceUrl) : "",
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
    }
}
