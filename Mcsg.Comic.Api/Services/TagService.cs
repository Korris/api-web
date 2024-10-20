using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Constants;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Interfaces;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;

public partial class TagService : ITagService
{
    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<ComicTagPost> _tagPostRepository;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly ISmartLookupService _smartLookupService;
    private readonly IRepository<ComicPost> _postRepository;
    private readonly IConfiguration _configuration;

    public TagService(
        IMcsgContext context,
        ICurrentUserService currentUserService,
        IUnitOfWork unitOfWork,
        ISmartLookupService smartLookupService,
        IRepository<SmartLookup> smartLookupRepository,
        IRepository<ComicPost> postRepository,
        IConfiguration configuration)
    {
        _context = context;
        _currentUserService = currentUserService;
        _tagPostRepository = unitOfWork.GetRepository<ComicTagPost>();
        _tagRepository = unitOfWork.GetRepository<Tag>();
        _smartLookupRepository = smartLookupRepository;
        _smartLookupService = smartLookupService;
        _postRepository = postRepository;
        _configuration = configuration;
    }

    public async Task<List<string>> AddTagsToPost(Guid postId, List<string> tags, Guid userId)
    {
        // Find all tags associated with the post
        var qTagPost = _context.ComicTagPostAvailable.Where(p => p.PostId == postId);
        var tagsDb = await (from a in _context.TagAvailable
                            join b in qTagPost
                               on a.Id equals b.TagId into g
                            from b in g.DefaultIfEmpty()
                            where tags.Contains(a.Name + "")
                            select new TagViewDto
                            {
                                Id = a.Id,
                                Title = a.Title + "",
                                Name = a.Name + "",
                                PostId = b == null ? Guid.Empty : b.PostId
                            }).ToListAsync();

        // Find tags that need to be created
        var listTagNeedToCreate = tags.Except(tagsDb.Select(x => x.Name)).ToList();

        // Add new tags (if any) and get their IDs
        var listTagAddToPost = listTagNeedToCreate.Count > 0 ? await AddNewTags(listTagNeedToCreate, userId) : [];

        // Add existing tags (not already associated with the post)
        listTagAddToPost.AddRange(tagsDb.Where(x => x.PostId != postId).Select(x => x.Id).ToList());
        await AddTagsToPost(postId, listTagAddToPost, userId);

        // Publish for smart lookup calculation
        await _smartLookupService.CalculateSmartLookupForTagAsync(tags);

        await _context.SaveChangesAsync(default);

        return tags;
    }

    public async Task<List<string>> UpdateTagsToPost(Guid postId, List<string>? tags, Guid userId)
    {
        // check tags null 
        if (tags == null)
        {
            await _context.ComicTagPosts
                .Where(p => p.PostId == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsDelete, true));
        }
        else
        {
            // Validate tags
            tags.ForEach(t => { t = ValidateTag(t); });

            // Find all tag with post (TagPost)
            var tagsPostDb = await GetTagsByPostIdAsync(postId);

            // update tag to post
            var tagsToUpdateShow = tags.Intersect(tagsPostDb.Select(x => x.Name)).ToList();
            var tagsToUpdateHidden = tagsPostDb.Select(x => x.Name).Except(tags).ToList();

            var listTagNeedToAdd = new List<Guid>();

            // Find all tag in database
            var tagsDb = await _context.TagAvailable.Where(p => tags.Contains(p.Name + "")).ToListAsync();

            // Check if tag not created, should create tag first.
            var listTagNotCreated = tags.Except(tagsDb.Select(x => x.Name)).ToList();
            if (listTagNotCreated.Count > 0)
            {
                var listNewTagId = await AddNewTags(listTagNotCreated, userId);
                listTagNeedToAdd.AddRange(listNewTagId);
            }

            // add tag to post
            var listTagExistNotAdd = tags.Except(tagsPostDb.Select(x => x.Name)).ToList();
            if (listTagExistNotAdd.Count > 0)
            {
                var idTagExist = _context.TagAvailable.Where(p => listTagExistNotAdd.Contains(p.Name)).Select(x => x.Id).ToArray();
                listTagNeedToAdd.AddRange(idTagExist);
            }

            var idTagsToUpdateShow = await _context.TagAvailable
                .Where(x => tagsToUpdateShow.Contains(x.Name))
                .Select(x => x.Id)
                .ToListAsync();

            await _context.ComicTagPosts
                .Where(p => idTagsToUpdateShow.Contains(p.TagId) && p.PostId == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsDelete, false));

            // Update tags to hide
            var idTagsToUpdateHidden = await _context.TagAvailable
                .Where(x => tagsToUpdateHidden.Contains(x.Name))
                .Select(x => x.Id)
                .ToListAsync();

            await _context.ComicTagPosts
                .Where(p => idTagsToUpdateHidden.Contains(p.TagId) && p.PostId == postId)
                .ExecuteUpdateAsync(p => p.SetProperty(x => x.IsDelete, true));

            await AddTagsToPost(postId, listTagNeedToAdd, userId);
            // Publish for smart lookup calculation
            await _smartLookupService.CalculateSmartLookupForTagAsync(tags);
        }

        await _context.SaveChangesAsync(default);
        return tags;
    }

    public async Task<IEnumerable<TagSuggestView>> GetSuggestTags(TagSuggestR tagSuggestReq)
    {
        string tagSearch = string.IsNullOrWhiteSpace(tagSuggestReq.Text) ? "" : ValidateTag(tagSuggestReq.Text);

        var tagsDb = await _tagRepository
                .Connection.QueryAsync<TagSuggestView>(GetSuggestTagsByNameQuery, new
                {
                    TagSearch = "%" + tagSearch + "%",
                    PageSize = Common.Core.Constants.Setting.PostConfig.TagSuggestMaxLength
                });
        return tagsDb;
    }

    public async Task<PagedResponse<PopularTagResponse>> GetPopularTags(TagPopularR popularTagReq)
    {
        var query = GetPopularTagsQuery;
        if (popularTagReq.PostType == null)
        {
            query = query.Replace("[AddPostType]", "");
        }
        else
        {
            query = query.Replace("[AddPostType]", @" post.""Type"" = @PostType AND ");
        }
        var offset = popularTagReq.PageSize * (popularTagReq.PageNumber - 1);
        var multipleQuery = await _tagRepository.Connection.
                            QueryMultipleAsync(query, new
                            {
                                PageSize = popularTagReq.PageSize,
                                Offet = offset,
                                PostType = popularTagReq.PostType
                            });

        var resDto = await multipleQuery.ReadAsync<PopularTagResponse>().ConfigureAwait(false);
        var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);
        var response = new PagedResponse<PopularTagResponse>(totalItems, popularTagReq.PageNumber, popularTagReq.PageSize);
        response.Items = resDto;

        return response;

    }

    public async Task<PagedResponse<TodayTrendingTagResponse>> GetTodayTrendingTags(TagTodayTrendingR todayTrendingTagReq)
    {
        int.TryParse(_configuration["TodayTrending:FetchDataTimes"], out var getdataTimes);
        int.TryParse(_configuration["TodayTrending:Days"], out var days);
        var offset = todayTrendingTagReq.PageSize * (todayTrendingTagReq.PageNumber - 1);
        var toDate = DateTime.UtcNow.Date.EndOfDay();
        var fromDate = toDate.AddDays(-days).BeginOfDay();

        IEnumerable<TodayTrendingTagResponse> resDto = null;
        PagedResponse<TodayTrendingTagResponse> response = null;
        var times = 1;
        do
        {
            var query = GetTodayTrendingTagsQuery;
            if (todayTrendingTagReq.PostType == null)
            {
                query = query.Replace("[AddPostType]", "");
            }
            else
            {
                query = query.Replace("[AddPostType]", @" AND post.""Type"" = @PostType ");
            }
            var multipleQuery = await _tagRepository.Connection.
                QueryMultipleAsync(query, new
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    PageSize = todayTrendingTagReq.PageSize,
                    Offet = offset,
                    PostType = todayTrendingTagReq.PostType
                });
            resDto = await multipleQuery.ReadAsync<TodayTrendingTagResponse>().ConfigureAwait(false);
            var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);
            if (resDto.Any())
            {
                response = new PagedResponse<TodayTrendingTagResponse>(totalItems, todayTrendingTagReq.PageNumber, todayTrendingTagReq.PageSize);
                response.Items = resDto;
            }
            else
            {
                times++;
                toDate = fromDate.AddMilliseconds(-1);
                fromDate = toDate.AddDays(-days).Date;
            }
        }
        while (times <= getdataTimes && !resDto.Any());

        if (response == null)
        {
            response = new PagedResponse<TodayTrendingTagResponse>(0, todayTrendingTagReq.PageNumber, todayTrendingTagReq.PageSize);
        }
        return response;

    }

    /// <summary>
    /// Find all tag with post (TagPost)
    /// </summary>
    /// <param name="postId"></param>
    /// <returns></returns>
    public async Task<List<TagViewDto>> GetTagsByPostIdAsync(Guid postId)
    {
        return await (from a in _context.TagAvailable
                      join b in _context.ComicTagPosts
                          on a.Id equals b.TagId
                      where b.PostId == postId
                      select new TagViewDto
                      {
                          Title = a.Title + "",
                          Name = a.Name + "",
                          Id = a.Id,
                          PostId = b.PostId
                      }).ToListAsync();
    }

    private async Task<List<Guid>> AddNewTags(List<string> tagNames, Guid userId)
    {
        var tagsToInsert = new List<Tag>();
        var smartLookupInserts = new List<SmartLookup>();

        foreach (var i in tagNames)
        {
            tagsToInsert.Add(new Tag
            {
                AuthorId = userId,
                CreatedBy = userId,
                ModifiedBy = userId,
                Name = i.Trim(),
                Title = i,
                IsDelete = false,
            });
            smartLookupInserts.Add(new SmartLookup
            {
                CountCriteria = 0,
                Keyword = i,
                KeywordType = LookupKeywordType.Tag
            });
        }

        await _context.Tags.AddRangeAsync(tagsToInsert);
        await _context.SmartLookups.AddRangeAsync(smartLookupInserts);

        return tagsToInsert.Select(x => x.Id).ToList();
    }

    private async Task<IEnumerable<Guid>> AddTagsToPost(Guid postId, List<Guid> tagIds, Guid userId)
    {
        var tagPostsToInsert = new List<ComicTagPost>();

        foreach (var i in tagIds)
        {
            tagPostsToInsert.Add(new ComicTagPost
            {
                IsDelete = false,
                PostId = postId,
                TagId = i,
                CreatedBy = userId,
                ModifiedBy = userId
            });
        }

        await _context.ComicTagPosts.AddRangeAsync(tagPostsToInsert);

        return tagPostsToInsert.Select(x => x.Id).ToList();
    }

    private string ValidateTag(string tag)
    {
        tag = (tag + "").TrimStart('#');

        if (tag.Length > Common.Core.Constants.Setting.PostConfig.TagMaxLength)
        {
            throw new ArgumentException(ErrorCodes.PortalTagNameNotValidLength, string.Format(ErrorMessage.TagNameNotValidLength, tag));
        }

        // Hash tag without any special character, except underscore, number and character
        if (!Regex.IsMatch(tag, Regular.Tag))
        {
            throw new ArgumentException(ErrorCodes.PortalTagNameNotValid, string.Format(ErrorMessage.TagNameNotValid, tag));
        }

        return tag.ToLower();
    }

    public async Task<PagedResponse<TagSearchResponse>> SearchTagbyKeyword(TagSearchR input)
    {
        PagedResponse<TagSearchResponse> results;
        if (string.IsNullOrWhiteSpace(input.Name))
        {
            return new PagedResponse<TagSearchResponse>(0);
        }
        var keywords = input.Name.ToLower().Split(' ');
        var query = $@"
                                SELECT t.""Name"",COUNT( t.""Id"")   from ""Tags"" t 
                                LEFT JOIN comic.""ComicTagPosts"" tp  
                                ON tp.""TagId""  = t.""Id"" 
                                LEFT JOIN ""comic"".""ComicPosts""  p 
                                ON p.""Id""  = tp.""PostId"" 
                                WHERE t.""IsDelete"" = false 
                                AND tp.""IsDelete"" = false 
                                [QueryCondition]
                                GROUP BY t.""Name"", t.""Id""
                                ORDER BY Count DESC
                                OFFSET @Offset
                                LIMIT @PageSize;
                                
                                SELECT COUNT(*) AS TotalItems
                                FROM (
                                    SELECT distinct  t.""Id""
                                    FROM ""comic"".""ComicPosts""  post
                                    INNER JOIN comic.""ComicTagPosts"" tagpost ON post.""Id"" = tagpost.""PostId"" 
                                    INNER JOIN ""Tags"" t ON tagpost.""TagId"" = t.""Id"" 
                                    WHERE  post.""IsDelete"" = false AND tagpost.""IsDelete"" = false 
                                    [QueryCondition]
                                    ) q";

        bool first = true;
        var queryCondition = "";
        foreach (var word in keywords)
        {
            if (first)
            {
                queryCondition += $@"AND t.""Name"" ILIKE '%{word}%'";
                first = false;
            }
            else
            {
                queryCondition += $@"OR t.""Name"" ILIKE '%{word}%'";
            }
        }
        query = query.Replace("[QueryCondition]", queryCondition);
        var offset = input.PageSize * (input.PageNumber - 1);
        var multi = await _tagRepository.Connection.QueryMultipleAsync(query, new
        {
            Offset = offset,
            PageSize = input.PageSize
        });
        var items = await multi.ReadAsync<TagSearchResponse>().ConfigureAwait(false);
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items.Any())
        {
            results = new PagedResponse<TagSearchResponse>(totalItems, input.PageNumber, input.PageSize);
            results.Items = items;
        }
        else
        {
            results = new PagedResponse<TagSearchResponse>(0);
        }

        return results;
    }

    public async Task<IEnumerable<TagSearchResponse>> SearchTagsByName(TagSearchKeywordR request)
    {
        var query = string.Format(string.IsNullOrWhiteSpace(request.Keyword) ? SearchTagsRandomQuery : SearchTagsByNameQuery, _tagRepository.TableName);
        var tagNames = await _tagRepository.Connection.QueryAsync<TagSearchResponse>(query, new
        {
            ExactKeyword = request.Keyword,
            StartsWithKeyword = $"{request.Keyword}%",
            ContainsKeyword = $"%{request.Keyword}%"
        });

        return tagNames;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    #endregion
}
