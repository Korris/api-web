using AutoMapper;
using Dapper;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Responses;
using Enums;
using Extensions;
using Interfaces;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class CommentService : ICommentService
{
    private readonly IRepository<ComicPostComment> _postCommentRepository;
    private readonly IRepository<ComicSubPostComment> _subPostCommentRepository;
    private readonly IRepository<ComicSubPost> _subPostRepository;
    private readonly IRepository<ComicResource> _resourceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private IConfiguration _configuration;
    protected readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ISetting setting, IConfiguration configuration, IBusinessText businessBodyText)
    {
        _postCommentRepository = unitOfWork.GetRepository<ComicPostComment>();
        _subPostCommentRepository = unitOfWork.GetRepository<ComicSubPostComment>();
        _resourceRepository = unitOfWork.GetRepository<ComicResource>();
        _subPostRepository = unitOfWork.GetRepository<ComicSubPost>();
        _userRepository = unitOfWork.GetRepository<User>();
        _mentionRepository = unitOfWork.GetRepository<Mention>();
        _mapper = mapper;
        _setting = setting;
        _configuration = configuration;
        _businessText = businessBodyText;
    }

    public async Task<PagedResponse<CommentResponse>> GetLatestPostCommentInAsync(Guid postId)
    {
        PagedResponse<CommentResponse> response;

        var result = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<CommentQueryResult>(GetCommentByPostInHomePageQuery, new { PostId = postId });

        if (result != null)
        {
            response = new PagedResponse<CommentResponse>(1, 1, 1);
            var commentData = new CommentResponse()
            {
                Id = result.Id,
                AuthorId = result.AuthorId,
                AuthorName = result.AuthorName,
                UserAvatar = result.UserAvatar.ToPublicImageUrl(),
                Body = result.Body,
                PostId = result.PostId,
                ModifiedOn = result.ModifiedOn,
                ResourceHashId = result.ResourceHashId,
                ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(result.ResourceName, result.ResourceUrl) : "",
                GifId = result.GifId
            };

            commentData.Body = await _businessText.Process(commentData.Body);

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
                    ModifiedOn = result.ReplyLastModifiedDate,
                    ResourceHashId = result.ReplyResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(result.ReplyResourceName, result.ReplyResourceUrl) : "",
                    ParentId = commentData.Id,
                    GifId = result.ReplyGifId,
                    QuoteId = result?.ReplyQuoteId == Guid.Empty ? null : result.ReplyQuoteId
                };

                replyData.Body = await _businessText.Process(replyData.Body);

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
            response = new PagedResponse<CommentResponse>(0);
        }
        return response;
    }
    public async Task<PagedResponse<CommentResponse>> GetLatestSubPostCommentInAsync(Guid postId)
    {
        PagedResponse<CommentResponse> response;

        var result = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<CommentQueryResult>(GetCommentBySubPostQuery, new { PostId = postId });

        if (result != null)
        {
            response = new PagedResponse<CommentResponse>(1, 1, 1);
            var commentData = new CommentResponse()
            {
                Id = result.Id,
                AuthorId = result.AuthorId,
                AuthorName = result.AuthorName,
                UserAvatar = result.UserAvatar.ToPublicImageUrl(),
                Body = result.Body,
                PostId = result.PostId,
                ModifiedOn = result.ModifiedOn,
                ResourceHashId = result.ResourceHashId,
                ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(result.ResourceName, result.ResourceUrl) : "",
                GifId = result.GifId
            };

            commentData.Body = await _businessText.Process(commentData.Body);

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
                    ModifiedOn = result.ReplyLastModifiedDate,
                    ResourceHashId = result.ReplyResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(result.ReplyResourceName, result.ReplyResourceUrl) : "",
                    ParentId = commentData.Id,
                    GifId = result.ReplyGifId
                };

                replyData.Body = await _businessText.Process(replyData.Body);

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
            response = new PagedResponse<CommentResponse>(0);
        }
        return response;
    }

    public async Task<List<BasicCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input)
    {
        List<BasicCommentResponse> results;

        var query = string.Format(GetReplyByCommentIdQuery, input.IsSubPost ? _subPostCommentRepository.TableName : _postCommentRepository.TableName);
        var items = await _postCommentRepository.Connection.QueryAsync<BasicCommentResponse>(query, new { CommentId = input.CommentId });
        if (items != null && items.Count() > 0)
        {
            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });

            foreach (var item in items)
            {
                item.UserAvatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.UserAvatar);
                item.ResourceUrl = !string.IsNullOrWhiteSpace(item.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(item.ResourceName, item.ResourceUrl) : "";
                if (mentions != null && mentions.Any())
                {
                    var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                    item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                }

                item.Body = await _businessText.Process(item.Body, profiles);
            }

            results = items.ToList();
            return results;
        }
        else
        {
            return null;
        }
    }

    public async Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR input)
    {
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
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });

            foreach (var item in items)
            {
                item.UserAvatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.UserAvatar);
                item.ResourceUrl = !string.IsNullOrWhiteSpace(item.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(item.ResourceName, item.ResourceUrl) : "";

                if (mentions != null && mentions.Any())
                {
                    var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                    item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                }

                item.Body = await _businessText.Process(item.Body, profiles);
            }

            results = new CommentPagedResults<MostReactionCommentResponse>(totalItems, input.PageNumber, input.PageSize);
            results.Items = items;
            results.TotalComments = await _postCommentRepository.Connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = input.HashPostId });
        }
        else
        {
            results = new CommentPagedResults<MostReactionCommentResponse>(0);
        }

        return results;
    }

    public async Task<CommentPagedResults<CommentResponse>> GetCommentsOfPostAsync(CommentLoadR request)
    {
        if (string.IsNullOrWhiteSpace(request.OrderBy))
        {
            request.OrderBy = "ModifiedOn";
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
                    UserName = comModel.UserName,
                    UserAvatar = comModel.UserAvatar.ToPublicImageUrl(),
                    Body = comModel.Body,
                    ModifiedOn = comModel.ModifiedOn,
                    ResourceHashId = comModel.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(comModel.ResourceName, comModel.ResourceUrl) : "",
                    GifId = comModel.GifId,
                    CustomNote = comModel.CustomNote
                };

                comment.Body = await _businessText.Process(comment.Body);

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
                        UserName = repModel.UserName,
                        UserAvatar = repModel.UserAvatar.ToPublicImageUrl(),
                        Body = repModel.Body,
                        ModifiedOn = repModel.ModifiedOn,
                        ResourceHashId = repModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(repModel.ResourceName, repModel.ResourceUrl) : "",
                        ParentId = comment.Id,
                        GifId = repModel.GifId,
                        QuoteId = repModel.QuoteId,
                        CustomNote = repModel.CustomNote
                    };

                    replyData.Body = await _businessText.Process(replyData.Body);

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

    public async Task<CommentPagedResults<CommentResponse>> GetCommentsOfSubPostAsync(CommentLoadR request, PostType postType)
    {
        var query = GetCommentOfSubPostQuery;
        var postId = request.PostId;

        if (postType == PostType.Feed)
        {
            var totalSubPost = await _postCommentRepository.Connection.QueryFirstOrDefaultAsync<SubPostCountModel>(CountSubPostOfPostQuery, new { PostId = request.PostId });

            if (string.IsNullOrWhiteSpace(request.OrderBy))
            {
                request.OrderBy = "ModifiedOn";
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
                    ModifiedOn = comModel.ModifiedOn,
                    ResourceHashId = comModel.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(comModel.ResourceName, comModel.ResourceUrl) : "",
                    GifId = comModel.GifId,
                    CustomNote = comModel.CustomNote
                };

                comment.Body = await _businessText.Process(comment.Body);

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
                        ModifiedOn = repModel.ModifiedOn,
                        ResourceHashId = repModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? _setting.Minio.MediaApiUrl.GetMediaPath(repModel.ResourceName, repModel.ResourceUrl) : "",
                        ParentId = comment.Id,
                        GifId = repModel.GifId,
                        CustomNote = repModel.CustomNote
                    };

                    replyData.Body = await _businessText.Process(replyData.Body);

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

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    #endregion
}
