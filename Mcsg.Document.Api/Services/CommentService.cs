using AutoMapper;
using Dapper;

namespace Mcsg.Document.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
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
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="businessBodyText"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="mapper"></param>
    public CommentService(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessBodyText, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _businessText = businessBodyText;

        _postCommentRepository = unitOfWork.GetRepository<DocumentPostComment>();
        _subPostCommentRepository = unitOfWork.GetRepository<DocumentSubPostComment>();
        _mentionRepository = unitOfWork.GetRepository<Mention>();
        _mapper = mapper;
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
                ResourceUrl = await _sc.GetPublicUrl(result.ResourceUrl, result.BucketName, result.MinioInstance),
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
                    UserAvatar = result.ReplyUserAvatar,
                    Body = result.ReplyBody,
                    ModifiedOn = result.ReplyLastModifiedDate,
                    ResourceHashId = result.ReplyResourceHashId,
                    ResourceUrl = await _sc.GetPublicUrl(result.ReplyResourceUrl, result.BucketName, result.MinioInstance),
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
                UserAvatar = result.UserAvatar,
                Body = result.Body,
                PostId = result.PostId,
                ModifiedOn = result.ModifiedOn,
                ResourceHashId = result.ResourceHashId,
                ResourceUrl = await _sc.GetPublicUrl(result.ResourceUrl, result.BucketName, result.MinioInstance),
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
                    UserAvatar = result.ReplyUserAvatar,
                    Body = result.ReplyBody,
                    ModifiedOn = result.ReplyLastModifiedDate,
                    ResourceHashId = result.ReplyResourceHashId,
                    ResourceUrl = await _sc.GetPublicUrl(result.ReplyResourceUrl, result.BucketName, result.MinioInstance),
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

    public async Task<PagedResponse<MostReactionCommentResponse>> GetReplyByCommentId(CommentReplyByCommentR input)
    {
        try
        {
            PagedResponse<MostReactionCommentResponse> results;

            var query = string.Format(GetReplyByCommentIdQuery, input.IsSubPost ? _subPostCommentRepository.TableName : _postCommentRepository.TableName);
            var offset = input.PageSize * (input.PageNumber - 1);
            var multi = await _postCommentRepository.Connection.QueryMultipleAsync(query, new { input.CommentId, input.PageSize, Offset = offset });
            var items = await multi.ReadAsync<MostReactionCommentResponse>().ConfigureAwait(false);
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var body = "";
            foreach (var i in items)
            {
                body += i.Body + " ";
            }
            var profiles = await _businessText.GetProfiles(body);

            var mentions = await _mentionRepository.Connection.QueryAsync<UserMentionModel>(GetUserMentionsInComments, new { LocationIds = items.Select(p => p.Id).ToList() });

            var tableName = input.IsSubPost ? $@"document.""DocumentSubPostCommentReactions""" : $@"document.""DocumentPostCommentReactions""";
            var targetIds = items.Select(p => p.Id).ToList();
            var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, tableName), new
            {
                TargetIds = targetIds,
                UserId = input.UserId
            });

            foreach (var item in items)
            {
                item.ResourceUrl = await _sc.GetPublicUrl(item.ResourceUrl, item.BucketName, item.MinioInstance);
                if (mentions != null && mentions.Any())
                {
                    var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                    item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                }

                item.Body = await _businessText.Process(item.Body, profiles);
                var postCommentReaction = postCommentReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                if (postCommentReaction.Count > 0)
                {
                    MapReactionResponse(item, postCommentReaction);
                }
            }

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<MostReactionCommentResponse>(totalItems, input.PageNumber, input.PageSize);
                results.Items = items;
            }
            else
            {
                results = new PagedResponse<MostReactionCommentResponse>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<CommentPagedResults<MostReactionCommentResponse>> GetCommentWithMostReaction(CommentMostReactionR request)
    {
        if (string.IsNullOrWhiteSpace(request.HashPostId))
        {
            return new CommentPagedResults<MostReactionCommentResponse>(0);
        }
        CommentPagedResults<MostReactionCommentResponse> results;
        var offset = request.PageSize * (request.PageNumber - 1);
        var query = GetCommentWithMostReactionQuery;

        var multi = await _postCommentRepository
           .Connection.QueryMultipleAsync(query, new
           {
               HashId = request.HashPostId,
               PageSize = request.PageSize,
               Offset = offset
           });
        var items = await multi.ReadAsync<MostReactionCommentResponse>().ConfigureAwait(false);
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items != null && items.Count() > 0)
        {
            var postComment = items.Where(p => p.Order == null).ToList();
            var subPostComment = items.Where(p => p.Order != null).ToList();
            var queryPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"document.""DocumentPostCommentReactions""");
            var querySubPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"document.""DocumentSubPostCommentReactions""");

            var userId = request.UserId;
            var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(queryPostCommentReaction, new
            {
                TargetIds = postComment.Select(p => p.Id).ToList(),
                UserId = userId
            });

            var subPostCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(querySubPostCommentReaction, new
            {
                TargetIds = subPostComment.Select(p => p.Id).ToList(),
                UserId = userId
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
                item.ResourceUrl = await _sc.GetPublicUrl(item.ResourceUrl, item.BucketName, item.MinioInstance);

                if (mentions != null && mentions.Any())
                {
                    var userMentioneds = mentions.Where(x => x.LocationId == item.Id).ToList();
                    item.Mentions = _mapper.Map<List<UserMentionResponse>>(userMentioneds);
                }

                item.Body = await _businessText.Process(item.Body, profiles);
            }

            results = new CommentPagedResults<MostReactionCommentResponse>(totalItems, request.PageNumber, request.PageSize);
            results.Items = items;
            results.TotalComments = await _postCommentRepository.Connection.QueryFirstAsync<int>(GetTotalCommentQuery, new { HashId = request.HashPostId });
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

    private void MapReactionCommentResponse(CommentResponse item, List<CommentReactionResponseQuery> reactions)
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

    private void MapReactionReplyCommentResponse(ReplyData item, List<CommentReactionResponseQuery> reactions)
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

    private async Task<CommentResponse> MapCommentResponse(CommentQueryModel queryModel)
    {
        return new CommentResponse()
        {
            Id = queryModel.Id,
            PostId = queryModel.PostId,
            AuthorId = queryModel.AuthorId,
            AuthorName = queryModel.AuthorName,
            UserName = queryModel.UserName,
            UserAvatar = queryModel.UserAvatar,
            Body = await _businessText.Process(queryModel.Body),
            ModifiedOn = queryModel.ModifiedOn,
            ResourceHashId = queryModel.ResourceHashId,
            ResourceUrl = await _sc.GetPublicUrl(queryModel.ResourceUrl, queryModel.BucketName, queryModel.MinioInstance),
            GifId = queryModel.GifId,
            CustomNote = queryModel.CustomNote.ForLexical(),
            ReplyCount = queryModel.ReplyCount,
        };
    }

    private async Task<ReplyData> MapReplyCommentResponsel(CommentQueryModel queryModel)
    {
        return new ReplyData()
        {
            Id = queryModel.Id,
            AuthorId = queryModel.AuthorId,
            AuthorName = queryModel.AuthorName,
            UserName = queryModel.UserName,
            ParentId = queryModel.ParentId,
            UserAvatar = queryModel.UserAvatar,
            Body = await _businessText.Process(queryModel.Body),
            ModifiedOn = queryModel.ModifiedOn,
            ResourceHashId = queryModel.ResourceHashId,
            ResourceUrl = await _sc.GetPublicUrl(queryModel.ResourceUrl, queryModel.BucketName, queryModel.MinioInstance),
            GifId = queryModel.GifId,
            CustomNote = queryModel.CustomNote.ForLexical(),
            QuoteId = queryModel.QuoteId,
            PostId = queryModel.PostId
        };
    }

    public async Task<CommentResponse> GetCommentById(Guid commentId, bool isSubPost, Guid? userId, Guid? replyCommentId)
    {
        var query = GetCommentByIdQuery;
        query = query.Replace("@CommentSource", $@"document.""{(isSubPost ? "DocumentSubPostComments" : "DocumentPostComments")}""");
        var comModel = await _postCommentRepository.Connection.QueryFirstAsync<CommentQueryModel>(query, new { CommentId = commentId });
        var result = new CommentResponse();
        var replyCommentMapping = new ReplyData();
        var replyCommentQuoteMapping = new ReplyData();
        if (comModel != null)
        {
            result = await MapCommentResponse(comModel);

            var replies = new ReplyResponse()
            {
                TotalReply = comModel.ReplyCount
            };

            if (replyCommentId != null)
            {
                var replyCommentModel = await _postCommentRepository.Connection.QueryFirstAsync<CommentQueryModel>(query, new { CommentId = replyCommentId });
                if (replyCommentModel != null)
                {
                    replyCommentMapping = await MapReplyCommentResponsel(replyCommentModel);
                    replies.Data.Add(replyCommentMapping);
                    if (replyCommentModel.QuoteId != null)
                    {
                        var replyCommentQuoteModel = await _postCommentRepository.Connection.QueryFirstAsync<CommentQueryModel>(query, new { CommentId = replyCommentModel.QuoteId });
                        replyCommentQuoteMapping = await MapReplyCommentResponsel(replyCommentQuoteModel);
                        replies.Data.Add(replyCommentQuoteMapping);
                        replies.Data.Reverse();
                    }
                }
            }

            var targetIds = new List<Guid> { result.Id };

            if (replyCommentId != null)
            {
                targetIds.Add(replyCommentId.Value);
            }
            if (replyCommentQuoteMapping != null)
            {
                targetIds.Add(replyCommentQuoteMapping.Id);
            }

            var tableName = isSubPost ? $@"document.""DocumentSubPostCommentReactions""" : $@"document.""DocumentPostCommentReactions""";
            var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(string.Format(ReactionExtension.GetReactionByTargetIdsQuery, tableName), new
            {
                TargetIds = targetIds,
                UserId = userId
            });
            if (postCommentReactionResponse.Any())
            {
                var reactionOfComment = postCommentReactionResponse.Where(p => p.TargetId == result.Id).ToList();
                if (reactionOfComment.Count > 0)
                {
                    MapReactionCommentResponse(result, reactionOfComment);
                }

                if (replyCommentId != null)
                {
                    var reactionOfReplyComment = postCommentReactionResponse.Where(p => p.TargetId == replyCommentId).ToList();
                    if (reactionOfReplyComment.Count > 0)
                    {
                        MapReactionReplyCommentResponse(replyCommentMapping, reactionOfReplyComment);
                    }
                    if (replyCommentQuoteMapping != null)
                    {
                        var reactionOfReplyCommentQuote = postCommentReactionResponse.Where(p => p.TargetId == replyCommentQuoteMapping.Id).ToList();
                        if (reactionOfReplyCommentQuote.Count > 0)
                        {
                            MapReactionReplyCommentResponse(replyCommentQuoteMapping, reactionOfReplyCommentQuote);
                        }
                    }
                }
            }

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
                    ResourceUrl = await _sc.GetPublicUrl(comModel.ResourceUrl, comModel.BucketName, comModel.MinioInstance),
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
                        UserName = repModel.UserName,
                        UserAvatar = repModel.UserAvatar,
                        Body = repModel.Body,
                        ModifiedOn = repModel.ModifiedOn,
                        ResourceHashId = repModel.ResourceHashId,
                        ResourceUrl = await _sc.GetPublicUrl(repModel.ResourceUrl, repModel.BucketName, repModel.MinioInstance),
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
                    ResourceUrl = await _sc.GetPublicUrl(comModel.ResourceUrl, comModel.BucketName, comModel.MinioInstance),
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
        var queryPostCommentReaction = string.Format(ReactionExtension.GetReactionByTargetIdsQuery, $@"document.""DocumentSubPostCommentReactions""");
        var userId = request.UserId;

        var postCommentReactionResponse = await _postCommentRepository.Connection.QueryAsync<CommentReactionResponseQuery>(queryPostCommentReaction, new
        {
            TargetIds = comments.Select(p => p.Id).ToList(),
            UserId = userId
        });

        foreach (var comment in comments)
        {
            var postCommentReaction = postCommentReactionResponse.Where(p => p.TargetId == comment.Id).ToList();
            if (postCommentReaction.Count > 0)
            {
                MapReactionCommentResponse(comment, postCommentReaction);
            }
        }
        CommentPagedResults<CommentResponse> response = new CommentPagedResults<CommentResponse>(totalRecord, request.PageNumber, request.PageSize);
        response.Items = comments;
        response.TotalComments = totalComments;
        return response;
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Storage client
    /// </summary>
    private readonly IStorageClient _sc;

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    private readonly IRepository<DocumentPostComment> _postCommentRepository;
    private readonly IRepository<DocumentSubPostComment> _subPostCommentRepository;
    private readonly IRepository<Mention> _mentionRepository;
    private readonly IMapper _mapper;

    #endregion
}
