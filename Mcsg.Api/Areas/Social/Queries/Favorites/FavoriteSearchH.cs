using Dapper;
using Grpc.Net.Client;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Web;
using Mcsg.Api.Interfaces;

namespace Mcsg.Api.Areas.Social.Commands;

using Analytic.Application.Protos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Mcsg.Api.Areas.Social.Dtos;
using Mcsg.Api.Areas.Social.Extensions;
using Mcsg.Api.Areas.Social.Filters;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Models;
using Mcsg.Api.Areas.Social.Requests;
using static Common.SeedWork.Constants.Error;

/// <summary>
/// Handler
/// </summary>
public class FavoriteSearchH : BaseMinioH, IRequestHandler<FavoriteSearchR, SingleResponse>
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="businessBodyText"></param>
    /// <param name="feedService"></param>
    public FavoriteSearchH(IMcsgContext context, ISetting setting, IStorageClient sc, IBusinessText businessBodyText, IFeedService feedService) : base(context, setting, sc)
    {
        _businessText = businessBodyText;
        _feedService = feedService;
    }

    /// <summary>
    /// Handle
    /// </summary>
    /// <param name="request">Request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Return the result</returns>
    public async Task<SingleResponse> Handle(FavoriteSearchR request, CancellationToken cancellationToken)
    {
        var res = new SearchResponse(request.PageNum, request.PageSize, request.Paging);

        if (request.UserId == null)
        {
            return res.SetError(nameof(E109), E109);
        }
        var userId = request.UserId.Value;

        IQueryable<BasePost> qPost = _context.Available<ComicPost>().Where(p => p.Status == PostStatus.Public);
        var qFavorite = _context.Available<ComicPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId);

        var type = request.ServiceType.ToEnum(PostType.Comic);
        var schema = type == PostType.Feed ? "social" : type.ToString().ToLower();
        switch (type)
        {
            case PostType.Feed:
                qPost = _context.Available<SocialPost>().Where(p => p.Status == PostStatus.Public);
                qFavorite = _context.Available<SocialPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId);
                break;

            case PostType.Story:
                qPost = _context.Available<StoryPost>().Where(p => p.Status == PostStatus.Public);
                qFavorite = _context.Available<StoryPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId);
                break;

            case PostType.Document:
                qPost = _context.Available<DocumentPost>().Where(p => p.Status == PostStatus.Public);
                qFavorite = _context.Available<DocumentPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId);
                break;

            default:
                break;
        }

        #region -- Filter --
        string? keyword = null;
        string? mediaType = null;

        if (request.Filter != null)
        {
            keyword = request.Filter + "";
            var ft = keyword.ToInstNull<PostFilter.Search>();
            if (ft != null)
            {
                keyword = ft.Keyword;
                mediaType = ft.MediaType;
            }

            if (type == PostType.Feed && !string.IsNullOrWhiteSpace(mediaType))
            {
                var qSubPost = from post in qPost
                               join subpost in _context.Available<SocialSubPost>()
                               on post.Id equals subpost.PostId
                               select new { post, subpost.Id };

                var qResource = from s in qSubPost
                                join resource in _context.Available<SocialResource>()
                                on s.Id equals resource.SubPostId
                                select new { s.post, resource.Type };

                var resourceType = mediaType.ToEnum(ResourceType.Other);
                if (resourceType == ResourceType.Video || resourceType == ResourceType.Image)
                {
                    var postIds = await qResource.Where(p => p.Type == resourceType).Select(p => p.post.Id).ToListAsync(cancellationToken);
                    qPost = _context.Available<SocialPost>().WhereIf(postIds.Count > 0, p => postIds.Contains(p.Id));
                }

                if (resourceType == ResourceType.Link)
                {
                    qPost = from post in qPost
                            join metadata in _context.SocialMetaDatas.Where(p => !string.IsNullOrEmpty(p.Title))
                            on post.Id equals metadata.PostId
                            select post;
                }
            }
        }
        #endregion

        IEnumerable<FavoritePostByUserResponse> data = [];
        var q = from post in qPost
                join favorite in qFavorite on post.Id equals favorite
                select new { post.Id, post.Title, post.Body, post.CreatedOn, post.AuthorName };

        var fn = "comic.fw_favorite_post_by_user";
        var @params = "@PostIds, @Hides, @Status";

        // Sort by views
        if (request.Sort?.Any(s => s.Field.Equals("views", StringComparison.OrdinalIgnoreCase)) == true)
        {
            var isAscending = request.Sort[0].Direction.Equals("ASC", StringComparison.OrdinalIgnoreCase);
            var postfav = await GetFavoriteFrAna(type.ToString(), userId.ToString(), keyword, request.PageNum, request.PageSize, isAscending);
            res.TotalRecords = postfav.TotalRecords;

            var postIds = postfav.Items.Select(p => Guid.Parse(p.Id)).ToList();
            using (var connection = _context.Database.GetDbConnection())
            {
                var paramValues = new
                {
                    PostIds = postIds,
                    Hides = request.Hides,
                    Status = StatusUtils.PostStatusInt
                };

                data = await connection.QueryAsync<FavoritePostByUserResponse>(fn.ToFn("comic", schema, @params), paramValues);
            }

            return res.SetSuccess(data);
        }

        // Keyword
        if (!string.IsNullOrWhiteSpace(keyword))
        {
            if (type != PostType.Feed)
            {
                q = q.Where(p => string.IsNullOrWhiteSpace(keyword) || EF.Functions.ILike(
                EF.Functions.Unaccent((p.Title + "").ToLower()),
                EF.Functions.Unaccent($"%{keyword.ToLower()}%")));
            }
            else
            {
                q = q.Where(p => string.IsNullOrWhiteSpace(keyword) ||
                EF.Functions.ILike(EF.Functions.Unaccent((p.Body + "").ToLower()), EF.Functions.Unaccent($"%{keyword.ToLower()}%")) ||
                EF.Functions.ILike(EF.Functions.Unaccent((p.AuthorName + "").ToLower()), EF.Functions.Unaccent($"%{keyword.ToLower()}%")));
            }
        }

        // Paging
        res.TotalRecords = q.Count();
        if (request.Paging)
        {
            q = q.Sort(request.Sort).PageBy(request.Offset, request.PageSize);
        }

        // Result
        var finalPostIds = q.Select(p => p.Id).ToList();

        if (type == PostType.Feed)
        {
            var result = await GetFavoriteFeedAsync(request, finalPostIds, userId);

            return res.SetSuccess(result);
        }
        else
        {
            var paramValues = new
            {
                PostIds = finalPostIds,
                Hides = request.Hides,
                Status = StatusUtils.PostStatusInt
            };

            using (var connection = _context.Database.GetDbConnection())
            {
                data = await connection.QueryAsync<FavoritePostByUserResponse>(fn.ToFn("comic", schema, @params), paramValues);
            }

            return res.SetSuccess(data);
        }
    }

    /// <summary>
    /// GetFavoriteFrAna
    /// </summary>
    /// <param name="type"></param>
    /// <param name="userId"></param>
    /// <param name="keyword"></param>
    /// <param name="pageNumber"></param>
    /// <param name="pageSize"></param>
    /// <param name="isAscending"></param>
    /// <returns></returns>
    private async Task<FavoriteViewSearchRsp> GetFavoriteFrAna(string type, string userId, string? keyword, int pageNumber, int pageSize, bool isAscending)
    {
        var res = new FavoriteViewSearchRsp() { Success = true };

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Analytic.Analytic!);
            var client = new FavoriteViewProto.FavoriteViewProtoClient(channel);

            var request = new FavoriteViewSearchReq
            {
                PostType = type,
                UserId = userId,
                Keyword = keyword,
                PageNum = pageNumber,
                PageSize = pageSize,
                IsAscending = isAscending
            };

            var rsp = await client.SearchAsync(request);
            res.Message = rsp.Message;
            res.TotalRecords = rsp.TotalRecords;
            res.Items.AddRange(rsp.Items);
        }
        catch (Exception ex)
        {
            res.Message = ex.Message;
            ex.Message.LogError();
        }

        return res;
    }

    private async Task<List<FeedBoxResponse>> GetFavoriteFeedAsync(FavoriteSearchR req, List<Guid> ids, Guid userId)
    {
        var schema = "social";

        var fn = "social.fw_favorite_post_by_user";
        var @params = "@PostIds, @Hide, @PostStatus";

        var paramValues = new
        {
            PostIds = ids,
            Hide = req.Hides,
            PostStatus = StatusUtils.PostStatusInt
        };

        using (var connection = _context.Database.GetDbConnection())
        {
            var result = await connection.QueryAsync<FeedBoxQueryResponse>(fn.ToFn("social", schema, @params), paramValues);

            var postIds = await _context.Available<SocialPostFavorite>().Where(p => p.UserId == userId).Select(p => p.PostId).ToListAsync();

            if (result != null && result.Any())
            {
                var listFeedDetails = new List<FeedBoxResponse>();

                var body = "";
                foreach (var i in result)
                {
                    body += i.Body + " ";
                }
                var profiles = await _businessText.GetProfiles(body);

                foreach (var i in result)
                {
                    i.Body = await _businessText.Process(i.Body, profiles);
                    listFeedDetails.Add(MappingFeedBoxResponse(i, postIds, userId));
                }

                fn = "social.fw_reaction_by_target_ids";
                @params = "@TargetIds, @UserId";

                var reactionParamValues = new
                {
                    TargetIds = listFeedDetails.Select(p => p.Id).ToList(),
                    UserId = userId
                };

                var postReactionResponse = await connection.QueryAsync<CommentReactionResponseQuery>(fn.ToFn("social", schema, @params), reactionParamValues);

                foreach (var item in listFeedDetails)
                {
                    item.IsCensored = !req.IsAdministrator && req.UserName != item.UserName && item.Status == PostStatus.Inactive;
                    item.IsBlur = item.Status == PostStatus.Inactive;
                }

                var sharePostIds = listFeedDetails.Where(p => p.SharePostId.HasValue)
                                       .Select(p => new SharePostInput
                                       {
                                           Id = p.SharePostId.Value,
                                           Type = p.SharePostType ?? SharePostType.Feed
                                       })
                                       .ToList();

                var sharePosts = await _feedService.GetSharePosts(req, sharePostIds);
                if (sharePosts.Count > 0)
                {
                    foreach (var item in listFeedDetails)
                    {
                        var sharePost = sharePosts.FirstOrDefault(p => p.Id == item.SharePostId);
                        if (sharePost != null)
                        {
                            item.SharePost = sharePost;
                        }
                    }
                }

                if (postReactionResponse.Any())
                {
                    foreach (var item in listFeedDetails)
                    {
                        var postReaction = postReactionResponse.Where(p => p.TargetId == item.Id).ToList();
                        if (postReaction.Any())
                        {
                            MapReactionFeedBoxResponse(item, postReaction);
                        }
                    }
                }

                return listFeedDetails;
            }
            else
            {
                return [];
            }
        }
    }

    private FeedBoxResponse MappingFeedBoxResponse(FeedBoxQueryResponse res, List<Guid>? postId, Guid? userId)
    {
        var itemResponse = new FeedBoxResponse()
        {
            ThumbnailUrl = res.ThumbnailUrl,
            Body = HttpUtility.HtmlDecode(res.Body),
            CreatedOn = res.CreatedOn,
            HashId = res.HashId,
            Id = res.Id,
            ProfileId = res.ProfileId,
            UserId = res.UserId,
            MetaData = res.MetaDatas != null ? JsonConvert.DeserializeObject<MetaDataDto>(res.MetaDatas) : null,
            TotalResources = res.TotalResources,
            UserAvatar = res.UserAvatar,
            FullName = res.FullName,
            UserName = res.UserName,
            Resources = res.TotalResources > 0 && res.Resources != null ? JsonConvert.DeserializeObject<List<ResourceDto>>(res.Resources.ToString()) : new List<ResourceDto>(),
            Type = res.Type,
            CustomNote = res.CustomNote.ForLexical(),
            IsFavorite = postId == null ? false : postId.Contains(res.Id),
            IsCurrentUserAuthor = res.UserId == userId,
            Hide = res.Hide,
            Status = res.Status,
            SharePostId = res.SharePostId,
            SharePostType = res.SharePostType
        };
        itemResponse.MetaData.Description = HttpUtility.HtmlDecode(itemResponse.MetaData.Description);

        var link = res.Link != null ? JsonConvert.DeserializeObject<PostLinkFeedBoxResponse>(res.Link) : null;
        if (res.TotalResources > 0 && !string.IsNullOrEmpty(res.Resources))
        {
            itemResponse.Resources = [];
            var resourceResponses = JsonConvert.DeserializeObject<List<ResourceDto>>(res.Resources);

            // Ensure not null
            resourceResponses = resourceResponses?.Where(p => p != null).OrderBy(p => p.Order).ToList();
            if (resourceResponses == null)
            {
                resourceResponses = [];
            }

            foreach (var i in resourceResponses)
            {
                if (i == null)
                {
                    continue;
                }

                i.Url = _sc.GetCdnUrl(i.Url, i.BucketName, i.MinioInstance, i.Type);

                itemResponse.Resources.Add(i);
                itemResponse.SubPosts?.Add(new SubUploadFileDto
                {
                    Files = new List<UploadFileDto>() {
                        new UploadFileDto() {
                            Order = i.Order,
                            SubPostHashId = i.SubPostHashId,
                            HashId = i.HashId,
                            Height = i.Height,
                            Width = i.Width,
                            Url = i.Url,
                            Type = i.Type,
                            Name = i.Name
                        }
                    }
                });
            }
        }
        if (link != null)
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = link.HashId,
                Url = link.Url,
                Type = link.Type.ToDisplay()
            };
            itemResponse.Resources = new List<ResourceDto>()
                {
                    new ResourceDto()
                    {
                        HashId = link.HashId,
                        Url = link.Url,
                        Type = link.Type.ToResourceType()
                    }
                };
        }

        return itemResponse;
    }
    private void MapReactionFeedBoxResponse(FeedBoxResponse item, List<CommentReactionResponseQuery> reactions)
    {
        var currentUserReact = reactions.Where(p => p.ReactByCurrent > 0).FirstOrDefault();
        item.Reaction = new ReactionsResponse
        {
            TargetId = item.Id,
            CurrentUserReactType = currentUserReact?.Type,
            Reactions = reactions.Select(p => new ReactionResponse { Count = p.Count, Type = p.Type.Value }).ToList(),
            TotalReacts = reactions.Select(p => p.Count).Sum(),
            MostReactionType = reactions.OrderByDescending(p => p.Count).FirstOrDefault()?.Type
        };
    }

    #endregion

    #region -- Fields --

    /// <summary>
    /// Business text
    /// </summary>
    private readonly IBusinessText _businessText;

    /// <summary>
    /// Feed service
    /// </summary>
    private readonly IFeedService _feedService;

    #endregion
}
