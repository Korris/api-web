using AutoMapper;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Web;

namespace Mcsg.Story.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Enums;
using Extensions;
using Interfaces;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class FeedService : IFeedService
{
    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="sc"></param>
    /// <param name="postService"></param>
    /// <param name="soundService"></param>
    /// <param name="unitOfWork"></param>
    /// <param name="feedDisplayConfig"></param>
    /// <param name="mapper"></param>
    public FeedService(IMcsgContext context, ISetting setting, IStorageClient sc, IPostService postService, ISoundService soundService, IUnitOfWork unitOfWork, IOptionsMonitor<FeedDisplayConfig> feedDisplayConfig, IMapper mapper)
    {
        _context = context;
        _setting = setting;
        _sc = sc;
        _postService = postService;
        _soundService = soundService;

        _unitOfWork = unitOfWork;
        _postRepository = unitOfWork.GetRepository<StoryPost>();
        _feedDisplayConfig = feedDisplayConfig.CurrentValue;
        _mapper = mapper;
    }

    public async Task<PagedResponse<FeedDto>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);
            var date = DateTime.UtcNow.Date;

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(StoryPost.CreatedOn);
            }
            var query = "";
            if (loadFeedType == LoadFeedType.TRENDING || loadFeedType == LoadFeedType.HOT)
            {
                query = string.Format(GetAllFeedsWithTopCommentQuery, _postRepository.TableName, feedLoadReq.OrderBy);
                if (loadFeedType == LoadFeedType.HOT)
                {
                    int offsetDate = date.DayOfWeek - DayOfWeek.Monday;
                    DateTime lastMonday = date.AddDays(-offsetDate);
                    date = lastMonday;
                }
            }
            else
            {
                query = string.Format(GetAllFeedsQuery, _postRepository.TableName, feedLoadReq.OrderBy);
            }

            query = AddAdditionalFeedQuery(feedLoadReq, query, loadFeedType);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        Type = (int)PostType.Feed,
                        IsAccessPrivate = false,
                        PageSize = feedLoadReq.PageSize,
                        Offet = offset,
                        Date = date,
                        Status = PostStatus.Public,
                        DateOnly = DateOnly.FromDateTime(date)
                    });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();
            foreach (var item in items)
            {
                listItemResponse.Add(MappingFeedInListRespone(item));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                results.Items = listItemResponse;
            }
            else
            {
                results = new PagedResponse<FeedDto>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<PagedResponse<FeedDto>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(StoryPost.CreatedOn);
            }
            var query = string.Format(GetAllFeedsByTagQuery, _postRepository.TableName, feedLoadReq.OrderBy);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        PostType = (int)PostType.Feed,
                        IsAccessPrivate = false,
                        PageSize = feedLoadReq.PageSize,
                        Offet = offset,
                        TagName = tagName,
                        Hide = feedLoadReq.Hides
                    });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();
            foreach (var item in items)
            {
                listItemResponse.Add(MappingFeedInListRespone(item));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                results.Items = listItemResponse;
            }
            else
            {
                results = new PagedResponse<FeedDto>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public async Task<SubPostFeedResponse> GetFeedSubPostAsync(IdBaseR request)
    {
        var hashId = request.HashId;
        var userId = request.UserId;

        var query = $@"WITH SubPostsCount AS (
                            SELECT ""PostId"", 
                                   COUNT(*) AS total_subposts 
                            FROM ""story"".""StorySubPosts""
                            WHERE ""IsDelete"" = false
                            GROUP BY ""PostId""
                        ),
                        ResourceCount AS (
                            SELECT sp.""PostId"",
                            to_jsonb(array_agg(
                            json_build_object(
                                'Url', r.""Url"",
                                'MinioInstance', r.""MinioInstance"",
                                'Height', r.""Height"",
                                'Width', r.""Width"",
                                'BucketName', r.""BucketName"",
                                'Type', r.""Type"",
                                'Name', r.""Name"",
                                'Order',r.""Order"",
                                'HashId',r.""HashId"",
                                'SubPostHashId',sp.""HashId""
                   
                            )
                            )) AS Resources
                            FROM ""story"".""StorySubPosts"" sp
                            LEFT JOIN ""story"".""StoryResources"" r ON sp.""Id"" = r.""SubPostId""
                            WHERE sp.""IsDelete"" = false
                            GROUP BY sp.""PostId""
                        )
                        SELECT 
                            u.""ProfileName"" AS Fullname,
                            u.""ProfileId"",
                            u.""Avatar"" AS UserAvatar,
                            u.""Id"" AS UserId,
                            p.""HashId"",
                            sp.""Id"",
                            sp.""CreatedOn"", 
                            sp.""Body"",
                            sp.""CreatedBy"",
                            COALESCE(psb.""HashId"", (
                                SELECT ps.""HashId"" 
                                FROM ""story"".""StorySubPosts"" ps 
                                WHERE ps.""PostId"" = sp.""PostId"" 
                                  AND ps.""Order"" = sc.total_subposts
                                  AND ps.""IsDelete"" = false
                            )) AS PrevSubPostHashId,
                            COALESCE(asp.""HashId"", (
                                SELECT ps.""HashId"" 
                                FROM ""story"".""StorySubPosts"" ps 
                                WHERE ps.""PostId"" = sp.""PostId"" 
                                  AND ps.""Order"" = 1
                                  AND ps.""IsDelete"" = false
                            )) AS NextSubPostHashId,
                            rc.Resources as ""ResourcesStr""
                        FROM ""story"".""StorySubPosts"" sp
                        LEFT JOIN identity.""Users"" u ON u.""Id"" = sp.""UserId""
                        LEFT JOIN ""story"".""StoryPosts""  p ON p.""Id"" = sp.""PostId""
                        LEFT JOIN ""story"".""StorySubPosts"" psb ON sp.""PostId"" = psb.""PostId""
                         AND sp.""Order"" = psb.""Order"" + 1
                         AND psb.""IsDelete"" = false
                         LEFT JOIN ""story"".""StorySubPosts"" asp ON sp.""PostId"" = asp.""PostId""
                         AND sp.""Order"" = asp.""Order"" - 1
                         AND asp.""IsDelete"" = false
                         LEFT JOIN SubPostsCount sc ON sp.""PostId"" = sc.""PostId""
                         LEFT JOIN ResourceCount rc ON sp.""PostId"" = rc.""PostId""
                         WHERE sp.""HashId"" = @Id
                        AND sp.""IsDelete"" = false";

        var dataQuery = await _postRepository.Connection.QueryFirstOrDefaultAsync<SubPostFeedQuery>(query, new
        {
            Id = hashId
        });
        var resource = JsonConvert.DeserializeObject<List<ResourceDto>>(dataQuery.ResourcesStr);
        var data = _mapper.Map<SubPostFeedResponse>(dataQuery);
        data.Resources = resource;

        /// if only 1 Resource when click popup will show data of this Post instead of SubPost
        if (data.Resources.Count == 1)
        {
            var postData = await _postRepository.Connection.QueryFirstAsync<SubPostFeedResponse>($@"SELECT ""Body"",""Id"",""HashId"" from ""story"".""StoryPosts""  WHERE ""HashId"" =@Id", new { Id = data.HashId });
            data.Id = postData.Id;
            data.Body = postData.Body;
            data.HashId = postData.HashId;
            data.Body = HttpUtility.HtmlDecode(data.Body);
        }

        foreach (var item in data.Resources)
        {
            item.Url = await _sc.GetPublicUrl(item.Url, item.BucketName, item.MinioInstance);
        }

        data.SubPosts.Add(new SubUploadFileDto
        {
            Files = new List<UploadFileDto>()
                {
                    new UploadFileDto()
                    {
                        HashId = hashId,
                        Height = data.Height,
                        Width = data.Width,
                        Url = data.Url,
                        Type = data.ResourceType,
                        Name = data.ResourceName
                    }
                }
        });
        return data;
    }

    public async Task<FeedDto> GetFeedAsync(string hashId, Guid userId)
    {
        var query = string.Format(GetFeedQuery, _postRepository.TableName);

        FeedQueryDbDto dbFeed = null;
        await _postRepository
            .Connection.QueryAsync<FeedQueryDbDto, SubPostQueryDbDto, UploadFileQueryDbDto, MetaDataQueryDto, PostLinkDbDto, FeedQueryDbDto>(query,
            (feed, subpost, uploadfiles, meta, link) =>
            {
                if (dbFeed == null)
                {
                    dbFeed = feed;
                }
                if (subpost != null)
                {
                    if (dbFeed.SubPostDbs == null)
                        dbFeed.SubPostDbs = new List<SubPostQueryDbDto>();
                    if (uploadfiles != null)
                    {
                        if (subpost.FileDbs == null)
                            subpost.FileDbs = new List<UploadFileQueryDbDto>();
                        subpost.FileDbs.Add(uploadfiles);
                    }
                    dbFeed.SubPostDbs.Add(subpost);
                }
                if (meta != null)
                {
                    if (feed.MetaDataDb == null)
                        feed.MetaDataDb = meta;
                }
                if (link != null)
                {
                    if (feed.LinkDb == null)
                        feed.LinkDb = link;
                }
                return feed;
            },
            param: new
            {
                HashId = hashId,
                IsAccessPrivate = false,
            }, splitOn: "Id, Id, Id, Id, Id");

        // Will map later
        var sound = await _soundService.GetSoundByPostAsync(dbFeed.Id);

        //Add view
        if (dbFeed == null)
        {
            throw new NotFoundException(E204, M204);
        }

        if (dbFeed.Status == PostStatus.Inactive || (dbFeed.Status == PostStatus.Draft && dbFeed.UserId != userId))
        {
            throw new NotFoundException(E204, M204);
        }

        return MappingFeedRespone(dbFeed, sound);
    }

    public FeedBoxResponse MappingFeedBoxResponse(FeedBoxQueryResponse res, Guid? currentUserId)
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
            IsCurrentUserAuthor = res.UserId == currentUserId
        };
        var link = res.Link != null ? JsonConvert.DeserializeObject<PostLinkFeedBoxResponse>(res.Link) : null;
        if (res.TotalResources > 0 && !string.IsNullOrEmpty(res.Resources))
        {
            itemResponse.Resources = new List<ResourceDto>();
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
                itemResponse.SubPosts.Add(new SubUploadFileDto
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
        else if (link is not null)
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
                        Type = link.Type.ToResourceType(),
                }
                     };
        }

        return itemResponse;
    }

    public async Task<List<FeedBoxResponse>> GetFeedsByIds(PaginatedR request)
    {
        var param = new { HashIds = request.HashIds.Split(',').ToList() };
        var result = await _postRepository.Connection.QueryAsync<FeedBoxQueryResponse>(GetFeedBoxQuery, param);
        var userId = request.UserId;

        if (result != null && result.Any())
        {
            var listFeedDetails = new List<FeedBoxResponse>();

            foreach (var res in result)
            {
                listFeedDetails.Add(MappingFeedBoxResponse(res, userId));
            }

            return listFeedDetails;
        }
        else
        {
            return new List<FeedBoxResponse>(); // Trả về danh sách rỗng nếu không có kết quả
        }
    }

    public async Task<PagedResponse<FeedDto>> GetFeedByKeywordAsync(string keyWord, FeedSearchKeywordR feedLoadReq)
    {
        try
        {
            PagedResponse<FeedDto> results;
            var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

            if (feedLoadReq.OrderBy == null)
            {
                feedLoadReq.OrderBy = nameof(StoryPost.CreatedOn);
            }
            var query = string.Format(GetAllFeedByKeyword, _postRepository.TableName, feedLoadReq.OrderBy, keyWord);

            var multi = await _postRepository
                    .Connection.QueryMultipleAsync(query, new
                    {
                        PostType = (int)PostType.Feed,
                        IsAccessPrivate = false,
                        PageSize = feedLoadReq.PageSize,
                        Offet = offset
                    });
            var items = await multi.ReadAsync<FeedsListQueryDbDto>().ConfigureAwait(false);
            var listItemResponse = new List<FeedDto>();
            foreach (var item in items)
            {
                listItemResponse.Add(MappingFeedInListRespone(item));
            }
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            if (items != null && items.Count() > 0)
            {
                results = new PagedResponse<FeedDto>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                results.Items = listItemResponse;
            }
            else
            {
                results = new PagedResponse<FeedDto>(0);
            }
            return results;
        }
        catch (Exception ex)
        {
            throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
        }
    }

    public FeedDisplayConfig GetFeedDisplayConfig()
    {
        return _feedDisplayConfig;
    }

    public async Task<bool> DeleteFeedAsync(Guid postId)
    {
        return await _postService.Delete(postId);
    }

    public FeedDto MappingFeedInListRespone(FeedsListQueryDbDto item)
    {
        var itemResponse = new FeedDto()
        {
            Id = item.Id,
            Title = item.Title,
            HashId = item.HashId,
            UserId = item.UserId,
            UserName = item.UserName,
            FullName = item.ProfileName,
            ProfileId = item.ProfileId,
            ThumbnailUrl = item.ThumbnailUrl,
            CreatedOn = item.CreatedOn,
            TotalResource = item.TotalResource,
            Type = item.Type,
            Status = item.Status,
            UserAvatar = item.UserAvatar,
            Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0],
            CustomNote = item.CustomNote.ForLexical()
        };
        itemResponse.Body = HttpUtility.HtmlDecode(item.Body);
        #region Mapping with db query list
        if (item.TotalResource > 0 && !string.IsNullOrEmpty(item.SubPostResourceStr))
        {
            itemResponse.Resources = new List<ResourceDto>();
            var resourceResponses = JsonConvert.DeserializeObject<List<ResourceDto>>(item.SubPostResourceStr);

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
            }
        }
        else if (!string.IsNullOrEmpty(item.LinkUrl))
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = item.LinkHashId ?? "",
                Url = item.LinkUrl ?? "",
                Type = item.LinkType.ToDisplay()
            };
            itemResponse.Resources = new List<ResourceDto>()
                {
                    new ResourceDto()
                    {
                        HashId = item.LinkHashId,
                        Url = item.LinkUrl ?? "",
                        Type = item.LinkType.ToResourceType()
                    }
                };
        }
        if (item.MetaTitle != null && item.MetaDomain != null)
        {
            itemResponse.MetaData = new MetaDataDto
            {
                Description = !string.IsNullOrWhiteSpace(item.MetaDescription) ? HttpUtility.HtmlDecode(item.MetaDescription) : "",
                Domain = item.MetaDomain ?? "",
                Title = !string.IsNullOrWhiteSpace(item.MetaTitle) ? HttpUtility.HtmlDecode(item.MetaTitle) : "",
                Url = item.MetaUrl ?? ""
            };
        }

        #endregion

        return itemResponse;
    }

    private FeedDto MappingFeedRespone(FeedQueryDbDto item, BackgroundMedia.SearchDto? sound)
    {
        if (item == null)
            return new FeedDto();

        var itemResponse = new FeedDto()
        {
            Id = item.Id,
            Title = item.Title,
            HashId = item.HashId,
            UserId = item.UserId,
            FullName = item.ProfileName,
            ThumbnailUrl = item.ThumbnailUrl,
            CreatedOn = item.CreatedOn,
            ProfileId = item.ProfileId,
            Type = item.Type,
            Status = item.Status,
            UserAvatar = item.UserAvatar,
            Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0],
            Body = HttpUtility.HtmlDecode(item.Body),
            CustomNote = item.CustomNote.ForLexical()
        };

        #region Mapping with db query single
        bool hasResources = false;
        if (item.SubPostDbs != null && item.SubPostDbs.Count > 0)
        {
            itemResponse.SubPosts = new List<SubUploadFileDto>();
            foreach (var subPostdb in item.SubPostDbs)
            {
                if (subPostdb == null || subPostdb.FileDbs == null)
                {
                    continue;
                }

                var fileDbs = subPostdb.FileDbs.FirstOrDefault();
                if (fileDbs == null)
                {
                    continue;
                }

                var url = _sc.GetCdnUrl(fileDbs.Url, fileDbs.BucketName, fileDbs.MinioInstance, fileDbs.Type);

                itemResponse.Resources.Add(new ResourceDto
                {
                    HashId = subPostdb.HashId,
                    Type = fileDbs.Type,
                    Width = fileDbs.Width,
                    Height = fileDbs.Height,
                    Url = url,
                    Order = fileDbs.Order,
                    SubPostHashId = subPostdb.HashId,

                });
                var subPostResponse = new SubUploadFileDto()
                {
                    Id = subPostdb.Id,
                    HashId = subPostdb?.HashId,
                    Title = subPostdb.Title,
                    Name = subPostdb.Name,
                    ThumbnailUrl = subPostdb.ThumbnailUrl,
                    Permission = subPostdb.Permission,
                    CreatedOn = subPostdb.CreatedOn,
                    PublishDate = subPostdb.PublishDate,
                    Status = subPostdb.Status,
                    Body = subPostdb.Body
                };
                if (subPostdb.FileDbs != null)
                {
                    hasResources = true;

                    subPostResponse.Files = subPostdb.FileDbs.Select(x =>
                    {
                        var resource = new UploadFileDto
                        {
                            HashId = subPostdb.HashId,
                            Url = _sc.GetCdnUrl(x.Url, x.BucketName, x.MinioInstance, x.Type),
                            Name = x.Name,
                            Type = x.Type,
                            Status = x.Status,
                            Width = x.Width,
                            Height = x.Height,
                            Order = x.Order
                        };

                        return resource;
                    }).ToList();
                }
                itemResponse.SubPosts.Add(subPostResponse);
            }
        }
        if (item.MetaDataDb != null)
        {
            itemResponse.MetaData = new MetaDataDto
            {
                Description = !string.IsNullOrWhiteSpace(item.MetaDataDb.Description) ? HttpUtility.HtmlDecode(item.MetaDataDb.Description) : "",
                Domain = item.MetaDataDb.Domain ?? "",
                Title = !string.IsNullOrWhiteSpace(item.MetaDataDb.Title) ? HttpUtility.HtmlDecode(item.MetaDataDb.Title) : "",
                Url = item.MetaDataDb.Url ?? ""
            };
        }
        if (item.LinkDb != null && !string.IsNullOrEmpty(item.LinkDb.HashId) && !hasResources)
        {
            itemResponse.Link = new PostLinkDto
            {
                HashId = item.LinkDb.HashId ?? "",
                Url = item.LinkDb.Url ?? "",
                Type = item.LinkDb.Type.ToDisplay()
            };
        }

        // Will map later
        itemResponse.BackgroundSound = sound;
        #endregion
        itemResponse.TotalResource = itemResponse.Resources.Count;
        return itemResponse;
    }
    private static string AddAdditionalFeedQuery(FeedLoadReq feedLoadReq, string query, LoadFeedType loadFeedType)
    {
        if (string.IsNullOrEmpty(feedLoadReq.UserName))
        {
            query = query.Replace("[AdditionalCondition]", "")
                .Replace("[AdditionalTotalQuery]", "")
                .Replace("[AdditionalTotalCondition]", "");
        }
        else
        {
            var additionalTotalQuery = @"INNER JOIN identity.""Users"" u ON u.""Id"" = p.""UserId"" ";
            var additionalTotalCondition = @$"AND u.""UserName"" = '{feedLoadReq.UserName}'";
            var additionalCondition = @$"AND u.""UserName"" = '{feedLoadReq.UserName}'";

            query = query.Replace("[AdditionalCondition]", additionalCondition)
                .Replace("[AdditionalTotalQuery]", additionalTotalQuery)
                .Replace("[AdditionalTotalCondition]", additionalTotalCondition);
        }
        return query;
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
    /// Post service
    /// </summary>
    private readonly IPostService _postService;

    /// <summary>
    /// Sound service
    /// </summary>
    private readonly ISoundService _soundService;

    private readonly IRepository<StoryPost> _postRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly FeedDisplayConfig _feedDisplayConfig;
    private readonly IMapper _mapper;

    #endregion
}
