using AutoMapper;
using Dapper;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Enums;
using Extensions;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class CommentService : ICommentService
{
    private readonly IRepository<SocialPostComment> _postCommentRepository;
    private readonly IRepository<SocialSubPostComment> _subPostCommentRepository;
    private readonly IRepository<SocialSubPost> _subPostRepository;
    private readonly IRepository<SocialResource> _resourceRepository;
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly ICurrentUserService _currentUserService;
    private IConfiguration _configuration;
    protected readonly IMapper _mapper;

    public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ISetting setting, IConfiguration configuration, IBusinessText businessBodyText, ICurrentUserService currentUserService)
    {
        _postCommentRepository = unitOfWork.GetRepository<SocialPostComment>();
        _subPostCommentRepository = unitOfWork.GetRepository<SocialSubPostComment>();
        _resourceRepository = unitOfWork.GetRepository<SocialResource>();
        _subPostRepository = unitOfWork.GetRepository<SocialSubPost>();
        _userRepository = unitOfWork.GetRepository<User>();
        _mentionRepository = unitOfWork.GetRepository<Mention>();
        _mapper = mapper;
        _setting = setting;
        _configuration = configuration;
        _businessText = businessBodyText;
        _currentUserService = currentUserService;
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
                UserAvatar = result.UserAvatar,
                Body = result.Body,
                PostId = result.PostId,
                ModifiedOn = result.ModifiedOn,
                ResourceHashId = result.ResourceHashId,
                ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(result.ResourceName, result.ResourceUrl) : "",
                GifId = result.GifId,
                CustomNote = result.CustomNote.ForLexical()
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
                    UserAvatar = result.ReplyUserAvatar,
                    Body = result.ReplyBody,
                    ModifiedOn = result.ReplyLastModifiedDate,
                    ResourceHashId = result.ReplyResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(result.ReplyResourceName, result.ReplyResourceUrl) : "",
                    ParentId = commentData.Id,
                    GifId = result.ReplyGifId,
                    QuoteId = result?.ReplyQuoteId == Guid.Empty ? null : result.ReplyQuoteId,
                    CustomNote = result.CustomNote.ForLexical()
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
                UserAvatar = result.UserAvatar,
                Body = result.Body,
                PostId = result.PostId,
                ModifiedOn = result.ModifiedOn,
                ResourceHashId = result.ResourceHashId,
                ResourceUrl = !string.IsNullOrWhiteSpace(result.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(result.ResourceName, result.ResourceUrl) : "",
                GifId = result.GifId,
                CustomNote = result.CustomNote.ForLexical()
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
                    UserAvatar = result.ReplyUserAvatar,
                    Body = result.ReplyBody,
                    ModifiedOn = result.ReplyLastModifiedDate,
                    ResourceHashId = result.ReplyResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(result.ReplyResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(result.ReplyResourceName, result.ReplyResourceUrl) : "",
                    ParentId = commentData.Id,
                    GifId = result.ReplyGifId,
                    CustomNote = result.CustomNote.ForLexical()
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

    public async Task<PagedResponse<BasicCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input)
    {
        try
        {
            PagedResponse<BasicCommentResponse> results;

            var query = string.Format(GetReplyByCommentIdQuery, input.IsSubPost ? _subPostCommentRepository.TableName : _postCommentRepository.TableName);
            var offset = input.PageSize * (input.PageNumber - 1);
            var multi = await _postCommentRepository.Connection.QueryMultipleAsync(query, new { CommentId = input.CommentId, PageSize = input.PageSize, Offset = offset });
            var items = await multi.ReadAsync<BasicCommentResponse>().ConfigureAwait(false);
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });

            foreach (var item in items)
            {
                item.ResourceUrl = !string.IsNullOrWhiteSpace(item.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(item.ResourceName, item.ResourceUrl) : "";
                if (mentions != null && mentions.Any())
                {
                    var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                    item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                }

                item.Body = await _businessText.Process(item.Body, profiles);
            }


            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<BasicCommentResponse>(totalItems, input.PageNumber, input.PageSize);
                results.Items = items;
            }
            else
            {
                results = new PagedResponse<BasicCommentResponse>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
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
            var postComment = items.Where(p => p.Order == null).ToList();
            var subPostComment = items.Where(p => p.Order != null).ToList();
            var queryPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialPostCommentReactions""");
            var querySubPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"social.""SocialSubPostCommentReactions""");

            var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(queryPostCommentReaction, new
            {
                TargetIds = postComment.Select(p => p.Id).ToList(),
                UserId = _currentUserService.Session?.UserId
            });

            var subPostCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(querySubPostCommentReaction, new
            {
                TargetIds = subPostComment.Select(p => p.Id).ToList(),
                UserId = _currentUserService.Session?.UserId
            });

            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });

            foreach (var item in items)
            {
                var postCommentReaction = postCommentReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postCommentReaction.Count > 0)
                {
                    MapReactionResponse(item, postCommentReaction);
                }
                var subPostCommentReaction = subPostCommentReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (subPostCommentReaction.Count > 0)
                {
                    MapReactionResponse(item, subPostCommentReaction);
                }
                item.ResourceUrl = !string.IsNullOrWhiteSpace(item.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(item.ResourceName, item.ResourceUrl) : "";

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

    public void MapReactionResponse(MostReactionCommentResponse item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(x => x.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Select(x => new ReactionResponse { Count = x.Count, Type = x.Type.Value }).ToList(),
            TotalReacts = reactions.Select(x => x.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault().Type
        };
    }

    public async Task<CommentResponse> GetCommentById(Guid commentId, bool isSubPost = false)
    {
        var query = GetCommentByIdQuery;
        query = query.Replace("@CommentSource", $@"social.""{(isSubPost ? "SocialSubPostComments" : "SocialPostComments")}""");
        var comModel = await _postCommentRepository.Connection.QueryFirstAsync<CommentQueryModel>(query, new { CommentId = commentId });
        var result = new CommentResponse();
        if (comModel != null)
        {
            result = new CommentResponse()
            {
                Id = comModel.Id,
                PostId = comModel.PostId,
                AuthorId = comModel.AuthorId,
                AuthorName = comModel.AuthorName,
                UserName = comModel.UserName,
                UserAvatar = comModel.UserAvatar,
                Body = comModel.Body,
                ModifiedOn = comModel.ModifiedOn,
                ResourceHashId = comModel.ResourceHashId,
                ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(comModel.ResourceName, comModel.ResourceUrl) : "",
                GifId = comModel.GifId,
                CustomNote = comModel.CustomNote.ForLexical(),
                ReplyCount = comModel.ReplyCount,
            };

            result.Body = await _businessText.Process(result.Body);
            var replies = new ReplyResponse()
            {
                TotalReply = comModel.ReplyCount
            };
            result.Replies = replies;
        }
        return result;
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
                    UserAvatar = comModel.UserAvatar,
                    Body = comModel.Body,
                    ModifiedOn = comModel.ModifiedOn,
                    ResourceHashId = comModel.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(comModel.ResourceName, comModel.ResourceUrl) : "",
                    GifId = comModel.GifId,
                    CustomNote = comModel.CustomNote.ForLexical()
                };

                comment.Body = await _businessText.Process(comment.Body);

                var replyModels = result.Where(x => x.ParentId == comModel.Id
                                    && x.CommentLevel == (int)CommentLevel.Reply).ToList();

                var replies = new ReplyResponse()
                {
                    TotalReply = replyModels.Count
                };
                comment.ReplyCount = replyModels.Count;
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
                    UserAvatar = comModel.UserAvatar,
                    Body = comModel.Body,
                    ModifiedOn = comModel.ModifiedOn,
                    ResourceHashId = comModel.ResourceHashId,
                    ResourceUrl = !string.IsNullOrWhiteSpace(comModel.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(comModel.ResourceName, comModel.ResourceUrl) : "",
                    GifId = comModel.GifId,
                    CustomNote = comModel.CustomNote.ForLexical()
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
                        UserAvatar = repModel.UserAvatar,
                        Body = repModel.Body,
                        ModifiedOn = repModel.ModifiedOn,
                        ResourceHashId = repModel.ResourceHashId,
                        ResourceUrl = !string.IsNullOrWhiteSpace(repModel.ResourceUrl) ? _setting.Api.Web.Media.GetMediaPath(repModel.ResourceName, repModel.ResourceUrl) : "",
                        ParentId = comment.Id,
                        GifId = repModel.GifId,
                        QuoteId = repModel.QuoteId,
                        CustomNote = repModel.CustomNote.ForLexical()
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
