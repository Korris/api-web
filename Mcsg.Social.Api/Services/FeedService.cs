using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Npgsql;

namespace Mcsg.Social.Api.Services
{
    using Api.Interfaces;
    using Common.Core.Interfaces;
    using Constants;
    using DTOs;
    using Enums;
    using Extensions;
    using Lib.Common.Constants;
    using Lib.Common.Exceptions;
    using Lib.Common.Helpers;
    using Lib.Common.Web.Security;
    using Lib.Data.Domain.Entities;
    using Lib.Data.Entities.Common;
    using Lib.Data.Enums;
    using Lib.Data.Repositories;
    using Lib.Data.Repositories.Interface;
    using Lib.Model.Enums;
    using Models;
    using Services.Interfaces;

    public partial class FeedService : IFeedService
    {
        private readonly IRepository<Post> _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagService _tagService;
        private readonly IPostService _postService;
        private readonly IFileService _fileService;
        private readonly ISmartCountService _smartCountService;
        private readonly IViewHistoryService _viewHistoryService;

        private readonly FileSetting _fileSetting;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMetaDataService _metaDataService;
        private readonly ISmartLookupService _smartLookupService;
        private readonly ISoundService _soundService;
        private readonly IConfiguration _configuration;
        private readonly FeedDisplayConfig _feedDisplayConfig;
        private readonly IPostLinkService _postLinkService;

        public FeedService(IUnitOfWork unitOfWork,
            ITagService tagService,
            IPostService postService,
            IFileService fileService,
            ISmartCountService smartCountService,
            IOptionsMonitor<FileSetting> fileSetting,
            IMetaDataService metaDataService,
            ICurrentUserService currentUserService,
            ISmartLookupService smartLookupService,
            IViewHistoryService viewHistoryService,
            IPostLinkService postLinkService,
            ISoundService soundService,
            IConfiguration configuration,
            IOptionsMonitor<FeedDisplayConfig> feedDisplayConfig,
            ISetting setting,
            IStorageClient sc)
        {
            _postRepository = unitOfWork.GetRepository<Post>();
            _unitOfWork = unitOfWork;
            _tagService = tagService;
            _postService = postService;
            _fileService = fileService;
            _smartCountService = smartCountService;
            _viewHistoryService = viewHistoryService;
            _fileSetting = fileSetting.CurrentValue;
            _metaDataService = metaDataService;
            _currentUserService = currentUserService;
            _smartLookupService = smartLookupService;
            _soundService = soundService;
            _configuration = configuration;
            _feedDisplayConfig = feedDisplayConfig.CurrentValue;
            _postLinkService = postLinkService;
            _setting = setting;
            _sc = sc;
        }

        #region Load data
        public async Task<PagedResults<FeedResponse>> GetFeedsAsync(FeedLoadReq feedLoadReq, LoadFeedType loadFeedType)
        {
            try
            {
                PagedResults<FeedResponse> results;
                var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);
                var date = DateTime.UtcNow.Date;

                if (feedLoadReq.OrderBy == null)
                {
                    feedLoadReq.OrderBy = nameof(Post.CreatedDate);
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
                            Type = (int)PostType.FEED,
                            IsAccessPrivate = false,
                            PageSize = feedLoadReq.PageSize,
                            Offet = offset,
                            Date = date,
                            Status = PostStatus.PUBLIC,
                            DateOnly = DateOnly.FromDateTime(date)
                        });
                var items = await multi.ReadAsync<FeedsListQueryDbResponse>().ConfigureAwait(false);
                var listItemResponse = new List<FeedResponse>();
                foreach (var item in items)
                {
                    listItemResponse.Add(MappingFeedInListRespone(item));
                }
                var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

                if (items != null && items.Count() > 0)
                {
                    results = new PagedResults<FeedResponse>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                    results.Items = listItemResponse;
                }
                else
                {
                    results = new PagedResults<FeedResponse>(0);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }
        public async Task<PagedResults<FeedResponse>> GetFeedsByTagAsync(string tagName, FeedLoadReq feedLoadReq)
        {
            try
            {
                PagedResults<FeedResponse> results;
                var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

                if (feedLoadReq.OrderBy == null)
                {
                    feedLoadReq.OrderBy = nameof(Post.CreatedDate);
                }
                var query = string.Format(GetAllFeedsByTagQuery, _postRepository.TableName, feedLoadReq.OrderBy);

                var multi = await _postRepository
                        .Connection.QueryMultipleAsync(query, new
                        {
                            PostType = (int)PostType.FEED,
                            IsAccessPrivate = false,
                            PageSize = feedLoadReq.PageSize,
                            Offet = offset,
                            TagName = tagName
                        });
                var items = await multi.ReadAsync<FeedsListQueryDbResponse>().ConfigureAwait(false);
                var listItemResponse = new List<FeedResponse>();
                foreach (var item in items)
                {
                    listItemResponse.Add(MappingFeedInListRespone(item));
                }
                var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

                if (items != null && items.Count() > 0)
                {
                    results = new PagedResults<FeedResponse>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                    results.Items = listItemResponse;
                }
                else
                {
                    results = new PagedResults<FeedResponse>(0);
                }
                return results;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }

        public async Task<SubPostFeedResponse> GetFeedSubPostAsync(string hashId)
        {
            var currentUserId = _currentUserService.Session?.UserId ?? Guid.Empty;
            var query = $@"WITH SubPostsCount AS (
                            SELECT ""PostId"", 
                            COUNT(*) AS total_subposts 
                            FROM ""SubPosts""
                            GROUP BY ""PostId""
                        )
                        SELECT 
                                    u.""ProfileName"" as Fullname,
                                    u.""ProfileId"" ,u.""Avatar"" as UserAvatar, 
                                    u.""Id"" as UserId,
                                    p.""HashId"",
                                    sp.""Id"",
                                    sp.""CreatedDate"", 
                                    sp.""Body"",
                                    sp.""CreatedBy"",
                                    r.""Url"" ,
                                    r.""Height"" ,
                                    r.""Width"" ,
                                    r.""ShareUrl"",
                                    r.""Type"" as ResourceType,
                                    r.""Name"" as ResourceName,
                                    COALESCE(psb.""HashId"", (SELECT ps.""HashId"" FROM ""SubPosts"" ps WHERE ps.""PostId"" = sp.""PostId"" AND ps.""Order"" = sc.total_subposts)) AS PrevSubPostHashId,
                       COALESCE(asp.""HashId"", (SELECT ps.""HashId"" FROM ""SubPosts"" ps WHERE ps.""PostId"" = sp.""PostId"" AND ps.""Order"" = 1)) AS NextSubPostHashId
                                    FROM ""SubPosts"" sp
                                    LEFT JOIN ""Users"" u 
                                    ON u.""Id""  = sp.""UserId"" 
                                    LEFT JOIN ""Posts"" p 
                                    ON p.""Id""  = sp.""PostId"" 
                                    LEFT JOIN ""Resources"" r 
                                    ON r.""SubPostId"" = sp.""Id"" 
                                    LEFT JOIN ""SubPosts"" psb 
                                    ON sp.""PostId"" = psb.""PostId"" 
                                    AND sp.""Order"" = psb.""Order"" + 1
                                    LEFT JOIN ""SubPosts"" asp 
                                    ON sp.""PostId"" = asp.""PostId"" 
                                    AND sp.""Order"" = asp.""Order"" - 1
                                    LEFT JOIN SubPostsCount sc
                                    ON sp.""PostId"" = sc.""PostId""
                                    WHERE sp.""HashId"" =@Id
                                    AND sp.""IsDelete"" = false";

            var data = await _postRepository.Connection.QueryFirstOrDefaultAsync<SubPostFeedResponse>(query, new
            {
                Id = hashId
            });
            if (data.ResourceType == ResourceType.VIDEO || data.ResourceType == ResourceType.AUDIO)
            {
                data.Url = await _sc.Strategy.PresignedGetObject(data.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null);
            }
            else
            {
                data.Url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, data.ResourceName, data.Url);
            }
            data.UserAvatar = string.IsNullOrEmpty(data.UserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, data.UserAvatar);
            data.SubPosts.Add(new SubPostResponse
            {
                Files = new List<UploadFileResponse>()
                {
                    new UploadFileResponse()
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

        public async Task<FeedResponse> GetFeedAsync(string hashId)
        {
            var query = string.Format(GetFeedQuery, _postRepository.TableName);

            FeedQueryDbResponse dbFeed = null;
            await _postRepository
                .Connection.QueryAsync<FeedQueryDbResponse, SubPostQueryDbResponse, UploadFileQueryDbResponse, MetaDataQueryDbResponse, PostLinkDb, FeedQueryDbResponse>(query,
                (feed, subpost, uploadfiles, meta, link) =>
                {
                    if (dbFeed == null)
                    {
                        dbFeed = feed;
                    }
                    if (subpost != null)
                    {
                        if (dbFeed.SubPostDbs == null)
                            dbFeed.SubPostDbs = new List<SubPostQueryDbResponse>();
                        if (uploadfiles != null)
                        {
                            if (subpost.FileDbs == null)
                                subpost.FileDbs = new List<UploadFileQueryDbResponse>();
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
                throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
            }
            var currentUserId = _currentUserService?.Session?.UserId;
            if (currentUserId != null)
            {
                //await _viewHistoryService.QueueAddView(currentUserId??Guid.Empty, dbFeed.Id, EntityType.POST, "", EntitySubType.SUB1);
            }
            if (dbFeed.Status == PostStatus.INACTIVE || (dbFeed.Status == PostStatus.DRAFT && dbFeed.UserId != currentUserId))
            {
                throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
            }

            return MappingFeedRespone(dbFeed, sound);
        }
        public FeedBoxResponse MappingFeedBoxResponse(FeedBoxQueryResponse res)
        {
            var itemResponse = new FeedBoxResponse()
            {
                ThumbnailUrl = res.ThumbnailUrl,
                Body = System.Web.HttpUtility.HtmlDecode(res.Body),
                CreatedDate = res.CreatedDate,
                HashId = res.HashId,
                Id = res.Id,
                ProfileId = res.ProfileId,
                UserId = res.UserId,
                MetaData = res.MetaDatas != null ? JsonConvert.DeserializeObject<MetaDataResponse>(res.MetaDatas) : null,
                TotalResources = res.TotalResources,
                UserAvatar = res.UserAvatar != null ? UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, res.UserAvatar) : null,
                FullName = res.FullName,
                Resources = res.TotalResources > 0 && res.Resources != null ? JsonConvert.DeserializeObject<List<ResourceResponse>>(res.Resources.ToString()) : new List<ResourceResponse>(),
                Type = res.Type,
            };
            var link = res.Link != null ? JsonConvert.DeserializeObject<PostLinkFeedBoxResponse>(res.Link) : null;
            if (res.TotalResources > 0 && !string.IsNullOrEmpty(res.Resources))
            {
                itemResponse.Resources = new List<ResourceResponse>();
                var resourceResponses = JsonConvert.DeserializeObject<List<ResourceResponse>>(res.Resources);
                if (resourceResponses != null && resourceResponses.Any())
                {
                    foreach (var resourceResponse in resourceResponses)
                    {
                        if (resourceResponse != null)
                        {
                            if (resourceResponse.Type == ResourceType.VIDEO || resourceResponse.Type == ResourceType.AUDIO)
                            {
                                resourceResponse.Url = _sc.Strategy.PresignedGetObject(resourceResponse.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                            }
                            else
                            {
                                resourceResponse.Url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, resourceResponse.Name, resourceResponse.Url);
                            }
                            itemResponse.Resources.Add(resourceResponse);
                        }
                    }
                }
            }
            else if (link is not null)
            {
                itemResponse.Link = new PostLinkResponse
                {
                    HashId = link.HashId,
                    Url = link.Url,
                    Type = link.Type.ToDisplay()
                };
                itemResponse.Resources = new List<ResourceResponse>()
                {
                    new ResourceResponse()
                    {
                        HashId = link.HashId,
                        Url = link.Url,
                        ShareUrl = link.Url,
                        Type = link.Type.ToResourceType(),
                }
                     };
            }
            return itemResponse;
        }
        public async Task<List<FeedBoxResponse>> GetFeedsByIds(string hashIds)
        {
            var param = new { HashIds = hashIds.Split(',').ToList() };
            var result = await _postRepository.Connection.QueryAsync<FeedBoxQueryResponse>(GetFeedBoxQuery, param);

            if (result != null && result.Any())
            {
                var listFeedDetails = new List<FeedBoxResponse>();

                foreach (var res in result)
                {
                    listFeedDetails.Add(MappingFeedBoxResponse(res));
                }

                return listFeedDetails;
            }
            else
            {
                return new List<FeedBoxResponse>(); // Trả về danh sách rỗng nếu không có kết quả
            }
        }

        public async Task<PagedResults<FeedResponse>> GetFeedByKeywordAsync(string keyWord, SearchKeywordReq feedLoadReq)
        {
            try
            {
                PagedResults<FeedResponse> results;
                var offset = feedLoadReq.PageSize * (feedLoadReq.PageNumber - 1);

                if (feedLoadReq.OrderBy == null)
                {
                    feedLoadReq.OrderBy = nameof(Post.CreatedDate);
                }
                var query = string.Format(GetAllFeedByKeyword, _postRepository.TableName, feedLoadReq.OrderBy, keyWord);

                var multi = await _postRepository
                        .Connection.QueryMultipleAsync(query, new
                        {
                            PostType = (int)PostType.FEED,
                            IsAccessPrivate = false,
                            PageSize = feedLoadReq.PageSize,
                            Offet = offset
                        });
                var items = await multi.ReadAsync<FeedsListQueryDbResponse>().ConfigureAwait(false);
                var listItemResponse = new List<FeedResponse>();
                foreach (var item in items)
                {
                    listItemResponse.Add(MappingFeedInListRespone(item));
                }
                var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

                if (items != null && items.Count() > 0)
                {
                    results = new PagedResults<FeedResponse>(totalItems, feedLoadReq.PageNumber, feedLoadReq.PageSize);
                    results.Items = listItemResponse;
                }
                else
                {
                    results = new PagedResults<FeedResponse>(0);
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
        #endregion

        #region Modify data
        public async Task<FeedResponse> PostFeedAsync(FeedPostReq feedPostReq)
        {
            var currentUserId = _currentUserService.Session.UserId;
            var currentProfileName = _currentUserService.Session.ProfileName;
            var currentUserName = _currentUserService.Session.UserName;
            var currentUserAvatar = _currentUserService.Session.UserAvatar;
            var currentUserAvatarUrl = string.IsNullOrEmpty(currentUserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, currentUserAvatar);

            var profileId = _currentUserService.Session.ProfileId;
            var hashId = StringGenerator.GetRandomString(SystemConfig.PostHashLength);
            if (string.IsNullOrEmpty(feedPostReq.Content))
            {
                throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
            }
            //Check first post
            var rewards = await _postService.CheckRewardsForPost(currentUserId, PostType.FEED);

            string cleanHtml = HtmlHelper.CleanHtml(feedPostReq.Content);
            var safePlainString = System.Web.HttpUtility.HtmlEncode(cleanHtml);
            var post = new Post()
            {
                Title = feedPostReq.Title,
                Type = PostType.FEED,
                HashId = hashId,
                UserId = currentUserId,
                Body = safePlainString,
                ThumbnailUrl = feedPostReq.ThumbnailUrl,
                AuthorName = currentProfileName,
                Status = PostStatus.PUBLIC,
                CreatedBy = currentUserId,
                CustomNote = feedPostReq.CustomNote
            };
            var result = new FeedPostResponse
            {
                Id = post.Id,
                Title = feedPostReq.Title,
                HashId = hashId,
                UserId = currentUserId,
                ThumbnailUrl = post.ThumbnailUrl,
                CreatedDate = DateTime.UtcNow,
                Status = PostStatus.PUBLIC,
                Body = cleanHtml,
                FullName = currentProfileName,
                AuthorName = currentProfileName,
                IsCurrentUserIsAuthor = true,
                ProfileId = profileId,
                UserAvatar = currentUserAvatarUrl,
                Rewards = rewards,
                CustomNote = post.CustomNote
            };
            try
            {
                await _postRepository.InsertAsync(post);
                if (feedPostReq.MetaData != null)
                {
                    feedPostReq.MetaData.Description = System.Web.HttpUtility.HtmlEncode(feedPostReq.MetaData.Description);
                    result.MetaData = await _metaDataService.AddMetaDataToObject<Post>(feedPostReq.MetaData, post.Id);
                }
                if (feedPostReq.Tags != null && feedPostReq.Tags.Count > 0)
                {
                    result.Tags = (await _tagService.AddTagsToPost(post.Id, feedPostReq.Tags)).ToArray();
                }
                if (feedPostReq.Files != null && feedPostReq.Files.Count > 0)
                {
                    result.SubPosts = (await _fileService.ProcessFeedFilesAsync(feedPostReq.Files, currentUserId, currentUserName, currentUserAvatarUrl, post.Id, post.HashId));
                    result.TotalResource = result.SubPosts?.Count ?? 0;
                }

                if (feedPostReq.SoundId != null && feedPostReq.SoundId != Guid.Empty)
                {
                    await _soundService.AddSoundAsync(post.Id, feedPostReq.SoundId.Value);
                }
                else
                {
                    await _soundService.RemoveSoundAsync(post.Id);
                }

                // Detech video link content feed
                if (feedPostReq.Files == null || feedPostReq.Files.Count == 0)
                {
                    result.Link = await _postLinkService.AddLinkAsync(post.Id, feedPostReq.Content);
                }
                else
                {
                    var removeLink = await _postLinkService.RemoveLinkAsync(post.Id);
                    if (removeLink)
                    {
                        result.Link = new PostLinkResponse();
                    }
                }
            }
            catch (PostgresException ex)
            {
                //Regenerate hash when dupplicate. Code == "23505"
                if (ex.TableName == $"{nameof(Post)}s")
                {
                    post.HashId = StringGenerator.GetRandomString(SystemConfig.PostHashLength);
                    await _postRepository.InsertAsync(post);
                }
                else
                {
                    _unitOfWork.RollbackTransaction();
                    throw ex;
                }
            }

            var resourceResponse = new List<ResourceResponse>();
            foreach (var subPost in result.SubPosts)
            {
                if (subPost.Files != null)
                {
                    foreach (var file in subPost.Files)
                    {
                        var resource = new ResourceResponse()
                        {
                            Name = file?.Name ?? "",
                            HashId = file?.HashId,
                            SubPostHashId = file?.SubPostHashId,
                            Status = file.Status,
                            Type = file.Type,
                            Url = file?.Url ?? "",
                            Width = file.Width,
                            ShareUrl = file.ShareUrl,
                            Height = file.Height,
                            Order = file.Order
                        };

                        if (resource.Type == ResourceType.AUDIO || resource.Type == ResourceType.VIDEO)
                        {
                            resource.Url = await _sc.Strategy.PresignedGetObject(resource.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null);
                        }
                        resourceResponse.Add(resource);
                    }
                }
            }
            result.Resources = resourceResponse;
            await _smartLookupService.CalculateSmartLookupWhenCreatePostAsync();
            return result;
        }
        public async Task<FeedResponse> UpdateFeedAsync(string hashId, UpdateFeedPostReq feedPostReq)
        {
            var currentUserId = _currentUserService.Session.UserId;
            var currentProfileName = _currentUserService.Session.ProfileName;
            var currentUserName = _currentUserService.Session.UserName;
            var currentUserAvatar = _currentUserService.Session.UserAvatar;
            var currentUserAvatarUrl = string.IsNullOrEmpty(currentUserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, currentUserAvatar);

            // GetSingleFeedQuery
            var profileId = _currentUserService.Session.ProfileId;
            if (string.IsNullOrEmpty(feedPostReq.Content))
            {
                throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
            }
            //Check first post
            var rewards = await _postService.CheckRewardsForPost(currentUserId, PostType.FEED);


            var query = string.Format(GetSingleFeedQuery, _postRepository.TableName);
            var post = await _postRepository.Connection.QueryFirstAsync<Post>
                (query, new { HashId = hashId });

            VerifyFeed(post, false);
            if (string.IsNullOrEmpty(feedPostReq.Content))
            {
                throw new BadRequestException(ErrorCodes.PortalFeedContentEmpty, ErrorMessage.FeedContentEmpty);
            }

            string cleanHtml = HtmlHelper.CleanHtml(feedPostReq.Content);
            var safePlainString = System.Web.HttpUtility.HtmlEncode(cleanHtml);

            post.Title = feedPostReq.Title;
            post.Body = safePlainString;
            post.ThumbnailUrl = feedPostReq.ThumbnailUrl;
            post.CustomNote = feedPostReq.CustomNote;
            post.LastModifiedBy = currentUserId;
            post.LastModifiedDate = DateTime.UtcNow;

            var result = new FeedPostResponse
            {
                Id = post.Id,
                Title = feedPostReq.Title,
                HashId = hashId,
                UserId = currentUserId,
                ThumbnailUrl = post.ThumbnailUrl,
                CreatedDate = DateTime.UtcNow,
                Status = PostStatus.PUBLIC,
                Body = cleanHtml,
                FullName = currentProfileName,
                AuthorName = currentProfileName,
                IsCurrentUserIsAuthor = true,
                ProfileId = profileId,
                UserAvatar = currentUserAvatarUrl,
                Rewards = rewards,
                CustomNote = post.CustomNote
            };
            try
            {
                await _postRepository.UpdateAsync(post);

                //if (feedPostReq.MetaData != null)
                //{
                //	feedPostReq.MetaData.Description = System.Web.HttpUtility.HtmlEncode(feedPostReq.MetaData.Description);
                //	result.MetaData = await _metaDataService.AddMetaDataToObject<Post>(feedPostReq.MetaData, post.Id);
                //}

                // Add tag to feed
                if (feedPostReq.Tags != null && feedPostReq.Tags.Count > 0)
                {
                    result.Tags = (await _tagService.UpdateTagsToPost(post.Id, feedPostReq.Tags)).ToArray();
                }

                // Add file to feed
                if (feedPostReq.Files != null && feedPostReq.Files.Count > 0)
                {
                    result.SubPosts = (await _fileService.UpdateFeedFilesAsync(feedPostReq.Files, currentUserId, currentUserName, currentUserAvatarUrl, post.Id, post.HashId));
                    result.TotalResource = result.SubPosts?.Count ?? 0;
                }
                else
                {
                    await _fileService.RemoveFileAsync(post.Id, currentUserName);
                    result.TotalResource = 0;
                }
                // Add background sound to feed
                if (feedPostReq.SoundId != null && feedPostReq.SoundId != Guid.Empty)
                {
                    await _soundService.AddSoundAsync(post.Id, feedPostReq.SoundId.Value);
                }
                else
                {
                    await _soundService.RemoveSoundAsync(post.Id);
                }

                // Detech video link content feed
                if (feedPostReq.Files == null || feedPostReq.Files.Count == 0)
                {
                    result.Link = await _postLinkService.AddLinkAsync(post.Id, feedPostReq.Content);
                }
                else
                {
                    var removeLink = await _postLinkService.RemoveLinkAsync(post.Id);
                    if (removeLink)
                    {
                        result.Link = new PostLinkResponse();
                    }
                }
            }
            catch (PostgresException ex)
            {
                //Regenerate hash when dupplicate. Code == "23505"
                if (ex.TableName == $"{nameof(Post)}s")
                {
                    post.HashId = StringGenerator.GetRandomString(SystemConfig.PostHashLength);
                    await _postRepository.UpdateAsync(post);
                }
                else
                {
                    _unitOfWork.RollbackTransaction();
                    throw ex;
                }
            }

            var resourceResponse = new List<ResourceResponse>();
            foreach (var subPost in result.SubPosts)
            {
                if (subPost.Files != null)
                {
                    foreach (var file in subPost.Files)
                    {
                        var resource = new ResourceResponse()
                        {
                            Name = file?.Name ?? "",
                            HashId = file?.HashId,
                            Status = file.Status,
                            Type = file.Type,
                            Url = file?.Url ?? "",
                            Width = file.Width,
                            ShareUrl = file.ShareUrl,
                            Height = file.Height,
                            Order = file.Order
                        };

                        if (resource.Type == ResourceType.AUDIO || resource.Type == ResourceType.VIDEO)
                        {
                            resource.Url = await _sc.Strategy.PresignedGetObject(resource.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null);
                        }
                        resourceResponse.Add(resource);
                    }
                }
            }
            result.Resources = resourceResponse;
            await _smartLookupService.CalculateSmartLookupWhenCreatePostAsync();
            return result;
        }
        public async Task<bool> DeleteFeedAsync(Guid postId)
        {
            return await _postService.Delete(postId);
        }
        public async Task<bool> ReportFeedAsync(ReportPostReq req)
        {
            return await _postService.ReportPostAsync(req);
        }
        #endregion

        #region Map Data
        public FeedResponse MappingFeedInListRespone(FeedsListQueryDbResponse item)
        {
            var itemResponse = new FeedResponse()
            {
                Id = item.Id,
                Title = item.Title,
                HashId = item.HashId,
                UserId = item.UserId,
                FullName = item.ProfileName,
                ProfileId = item.ProfileId,
                ThumbnailUrl = item.ThumbnailUrl,
                CreatedDate = item.CreatedDate,
                TotalResource = item.TotalResource,
                Type = item.Type,
                Status = item.Status,
                UserAvatar = string.IsNullOrEmpty(item.UserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.UserAvatar),
                Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0],
                CustomNote = item.CustomNote
            };
            itemResponse.Body = System.Web.HttpUtility.HtmlDecode(item.Body);
            #region Mapping with db query list
            if (item.TotalResource > 0 && !string.IsNullOrEmpty(item.SubPostResourceStr))
            {
                itemResponse.Resources = new List<ResourceResponse>();
                var resourceResponses = JsonConvert.DeserializeObject<List<ResourceResponse>>(item.SubPostResourceStr);
                foreach (var resourceResponse in resourceResponses)
                {
                    if (resourceResponse != null)
                    {
                        if (resourceResponse.Type == ResourceType.VIDEO || resourceResponse.Type == ResourceType.AUDIO)
                        {
                            resourceResponse.Url = _sc.Strategy.PresignedGetObject(resourceResponse.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                        }
                        else
                        {
                            resourceResponse.Url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, resourceResponse.Name, resourceResponse.Url);
                        }
                        itemResponse.Resources.Add(resourceResponse);
                    }
                }
            }
            else if (!string.IsNullOrEmpty(item.LinkUrl))
            {
                itemResponse.Link = new PostLinkResponse
                {
                    HashId = item.LinkHashId ?? "",
                    Url = item.LinkUrl ?? "",
                    Type = item.LinkType.ToDisplay()
                };
                itemResponse.Resources = new List<ResourceResponse>()
                {
                    new ResourceResponse()
                    {
                        HashId = item.LinkHashId,
                        Url = item.LinkUrl ?? "",
                        ShareUrl = item.LinkUrl ?? "",
                        Type = item.LinkType.ToResourceType()
                    }
                };
            }
            if (item.MetaTitle != null && item.MetaDomain != null)
            {
                itemResponse.MetaData = new MetaDataResponse
                {
                    Description = !string.IsNullOrWhiteSpace(item.MetaDescription) ? System.Web.HttpUtility.HtmlDecode(item.MetaDescription) : "",
                    Domain = item.MetaDomain ?? "",
                    Title = !string.IsNullOrWhiteSpace(item.MetaTitle) ? System.Web.HttpUtility.HtmlDecode(item.MetaTitle) : "",
                    Url = item.MetaUrl ?? ""
                };
            }

            #endregion

            return itemResponse;
        }
        #endregion

        #region Private method
        private FeedResponse MappingFeedRespone(FeedQueryDbResponse item, SoundDto? sound)
        {
            if (item == null)
                return new FeedResponse();

            var itemResponse = new FeedResponse()
            {
                Id = item.Id,
                Title = item.Title,
                HashId = item.HashId,
                UserId = item.UserId,
                FullName = item.ProfileName,
                ThumbnailUrl = item.ThumbnailUrl,
                CreatedDate = item.CreatedDate,
                ProfileId = item.ProfileId,
                Type = item.Type,
                Status = item.Status,
                UserAvatar = string.IsNullOrEmpty(item.UserAvatar) ? string.Empty : UrlHelper.GetPublicImageUrl(_setting.Minio.MediaApiUrl, item.UserAvatar),
                Tags = (item.Tags != null && item.Tags[0] != null) ? item.Tags : new string[0],
                Body = System.Web.HttpUtility.HtmlDecode(item.Body),
                CustomNote = item.CustomNote
            };

            #region Mapping with db query single
            bool hasResources = false;
            if (item.SubPostDbs != null && item.SubPostDbs.Count > 0)
            {
                itemResponse.SubPosts = new List<SubPostResponse>();
                foreach (var subPostdb in item.SubPostDbs)
                {
                    var fileDbs = subPostdb.FileDbs.FirstOrDefault();
                    var url = "";
                    if (fileDbs.Type == ResourceType.VIDEO || fileDbs.Type == ResourceType.AUDIO)
                    {
                        url = _sc.Strategy.PresignedGetObject(fileDbs.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                    }
                    else
                    {
                        url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, fileDbs.Name, fileDbs.Url);
                    }
                    itemResponse.Resources.Add(new ResourceResponse
                    {
                        HashId = subPostdb.HashId,
                        Type = fileDbs.Type,
                        Width = fileDbs.Width,
                        Height = fileDbs.Height,
                        Url = url,
                        Order = fileDbs.Order
                    });
                    var subPostResponse = new SubPostResponse()
                    {
                        Id = subPostdb.Id,
                        HashId = subPostdb?.HashId,
                        Title = subPostdb.Title,
                        Name = subPostdb.Name,
                        ThumbnailUrl = subPostdb.ThumbnailUrl,
                        Permission = subPostdb.Permission,
                        CreatedDate = subPostdb.CreatedDate,
                        PublishDate = subPostdb.PublishDate,
                        Status = subPostdb.Status,
                        Body = subPostdb.Body
                    };
                    if (subPostdb.FileDbs != null)
                    {
                        hasResources = true;

                        subPostResponse.Files = subPostdb.FileDbs.Select(x =>
                        {
                            var resource = new UploadFileResponse
                            {
                                HashId = subPostdb.HashId,
                                Url = UrlHelper.GetMediaPath(_setting.Minio.MediaApiUrl, x.Name, x.Url),
                                Name = x.Name,
                                ShareUrl = x.ShareUrl,
                                Type = x.Type,
                                Status = x.Status,
                                Width = x.Width,
                                Height = x.Height,
                                Order = x.Order
                            };
                            if (x.Type == ResourceType.AUDIO || x.Type == ResourceType.VIDEO)
                            {
                                resource.Url = _sc.Strategy.PresignedGetObject(x.ShareUrl, _setting.Minio.MaxExpiryInSeconds, null).GetAwaiter().GetResult();
                            }
                            return resource;
                        }).ToList();
                    }
                    itemResponse.SubPosts.Add(subPostResponse);
                }
            }
            if (item.MetaDataDb != null)
            {
                itemResponse.MetaData = new MetaDataResponse
                {
                    Description = !string.IsNullOrWhiteSpace(item.MetaDataDb.Description) ? System.Web.HttpUtility.HtmlDecode(item.MetaDataDb.Description) : "",
                    Domain = item.MetaDataDb.Domain ?? "",
                    Title = !string.IsNullOrWhiteSpace(item.MetaDataDb.Title) ? System.Web.HttpUtility.HtmlDecode(item.MetaDataDb.Title) : "",
                    Url = item.MetaDataDb.Url ?? ""
                };
            }
            if (item.LinkDb != null && !string.IsNullOrEmpty(item.LinkDb.HashId) && !hasResources)
            {
                itemResponse.Link = new PostLinkResponse
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
            if (string.IsNullOrEmpty(feedLoadReq.ProfileName))
            {
                query = query.Replace("[AdditionalCondition]", "")
                    .Replace("[AdditionalTotalQuery]", "")
                    .Replace("[AdditionalTotalCondition]", "");
            }
            else
            {
                var additionalTotalQuery = @"INNER JOIN ""Users"" u ON u.""Id"" = p.""UserId"" ";
                var additionalTotalCondition = @$"AND u.""ProfileName"" = '{feedLoadReq.ProfileName}'";
                var additionalCondition = @$"AND u.""ProfileName"" = '{feedLoadReq.ProfileName}'";

                query = query.Replace("[AdditionalCondition]", additionalCondition)
                    .Replace("[AdditionalTotalQuery]", additionalTotalQuery)
                    .Replace("[AdditionalTotalCondition]", additionalTotalCondition);
            }
            return query;
        }
        private void VerifyFeed(Post post, bool checkCompleted)
        {
            var currentUserId = _currentUserService.Session.UserId;
            if (post != null)
            {
                if (post.IsDelete)
                {
                    throw new BadRequestException(ApiErrorCode.POST_HAS_DELETED, ApiErrorMessage.POST_HAS_DELETED);
                }
                if (post.CreatedBy != currentUserId)
                {
                    //TODO check permission
                    //throw new ForbiddenAccessException(ApiErrorCode.USER_NOT_PERMISSION, ApiErrorMessage.USER_NOT_PERMISSION);
                }
                if (checkCompleted && (post.IsCompleted ?? false))
                {
                    throw new ForbiddenAccessException(ApiErrorCode.POST_HAS_COMPLETED, ApiErrorMessage.POST_HAS_COMPLETED);
                }
            }
            else
            {
                throw new NotFoundException(ApiErrorCode.POST_NOT_EXIST, ApiErrorMessage.POST_NOT_EXIST);
            }
        }
        #endregion

        #region -- Fields --

        /// <summary>
        /// Setting
        /// </summary>
        private readonly ISetting _setting;

        /// <summary>
        /// Storage client
        /// </summary>
        private readonly IStorageClient _sc;

        #endregion
    }
}
