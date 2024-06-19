using Dapper;
using Mcsg.Social.Api.Constants;
using Mcsg.Social.Api.DTOs;
using Mcsg.Social.Api.Extensions;
using Mcsg.Social.Api.Models;
using Mcsg.Social.Api.Models.Tag;
using Mcsg.Social.Api.Services.Interfaces;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using System.Text.RegularExpressions;

namespace Mcsg.Social.Api.Services
{
    public partial class TagService : ITagService
    {
        private readonly IRepository<Tag> _tagRepository;
        private readonly IRepository<TagPost> _tagPostRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<SmartLookup> _smartLookupRepository;
        private readonly ISmartLookupService _smartLookupService;
        private readonly IRepository<Post> _postRepository;
        private readonly IConfiguration _configuration;

        public TagService(
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ISmartLookupService smartLookupService,
            IRepository<SmartLookup> smartLookupRepository,
            IRepository<Post> postRepository,
            IConfiguration configuration)
        {
            _currentUserService = currentUserService;
            _tagPostRepository = unitOfWork.GetRepository<TagPost>();
            _tagRepository = unitOfWork.GetRepository<Tag>();
            _smartLookupRepository = smartLookupRepository;
            _smartLookupService = smartLookupService;
            _postRepository = postRepository;
            _configuration = configuration;
        }
        public async Task<List<string>> AddTagsToPost(Guid postId, List<string> tags)
        {
            // Validate tags
            tags = tags.Select(ValidateTag).ToList(); // Use Select for cleaner transformation

            // Find all tags associated with the post
            var query = string.Format(GetAllTagsByNameQuery, _tagRepository.TableName);
            var tagsDb = await _tagRepository.Connection
                .QueryAsync<TagView>(query, new { TagNames = tags, PostId = postId });

            // Find tags that need to be created
            var listTagNeedToCreate = tags.Except(tagsDb.Select(x => x.Name)).ToList();

            // Add new tags (if any) and get their IDs
            List<Guid> listTagAddToPost = listTagNeedToCreate.Count > 0
                ? await AddNewTags(listTagNeedToCreate)
                : new List<Guid>();

            // Add existing tags (not already associated with the post)
            listTagAddToPost.AddRange(
                tagsDb.Where(x => x.PostId != postId).Select(x => x.Id).ToList()
            );
            await AddTagsToPost(postId, listTagAddToPost);

            // Publish for smart lookup calculation
            await _smartLookupService.CalculateSmartLookupForTagAsync(tags);

            return tags;
        }
        public async Task<List<string>> UpdateTagsToPost(Guid postId, List<string> tags)
        {
            // Validate tags
            tags.ForEach(t => { t = ValidateTag(t); });

            // Find all tag in database
            var tagsDb = await _tagRepository
                    .Connection.QueryAsync<Tag>(GetTagsByTagNameQuery, new
                    {
                        TagNames = tags
                    });

            var listTagNeedToAdd = new List<Guid>();

            // Check if tag not created, should create tag first.
            var listTagNotCreated = tags.Except(tagsDb.Select(x => x.Name)).ToList();
            if (listTagNotCreated.Count > 0)
            {
                var listNewTagId = await AddNewTags(listTagNotCreated);
                listTagNeedToAdd.AddRange(listNewTagId);
            }

            // Find all tag with post (TagPost)
            var query = string.Format(GetAllTagsByPostQuery, _tagRepository.TableName);
            var tagsPostDb = await _tagRepository
                    .Connection.QueryAsync<TagView>(query, new
                    {
                        PostId = postId
                    });

            // Update tag to post
            var listTagExistNotAdd = tags.Except(tagsPostDb.Select(x => x.Name)).Except(listTagNotCreated.Select(y => y)).ToList();
            if (listTagExistNotAdd.Count > 0)
            {
                var tagExistNotAddIds = tagsDb.Where(x => listTagExistNotAdd.Contains(x.Name)).Select(x => x.Id).ToList();
                listTagNeedToAdd.AddRange(tagExistNotAddIds);
            }


            // Remove tag to post
            var listRemove = tagsPostDb.Where(x => !tags.Any(y => x.Name == y)).Select(x => x.Id).ToArray();
            if (listRemove.Length > 0)
            {
                await _tagRepository
                    .Connection.ExecuteAsync(DeleteTagPosts, new
                    {
                        PostId = postId,
                        TagPostIds = listRemove
                    });
            }

            await AddTagsToPost(postId, listTagNeedToAdd);

            //Publish to calculate smart lookup for tag
            await _smartLookupService.CalculateSmartLookupForTagAsync(tags);
            return tags;
        }

        public async Task<IEnumerable<TagSuggestView>> GetSuggestTags(TagSuggestReq tagSuggestReq)
        {
            string tagSearch = string.IsNullOrWhiteSpace(tagSuggestReq.Text) ? "" : ValidateTag(tagSuggestReq.Text);

            var tagsDb = await _tagRepository
                    .Connection.QueryAsync<TagSuggestView>(GetSuggestTagsByNameQuery, new
                    {
                        TagSearch = "%" + tagSearch + "%",
                        PageSize = SystemConfig.TagSuggestMaxLength
                    });
            return tagsDb;
        }

        public async Task<PagedResults<PopularTagResponse>> GetPopularTags(PopularTagReq popularTagReq)
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
            var response = new PagedResults<PopularTagResponse>(totalItems, popularTagReq.PageNumber, popularTagReq.PageSize);
            response.Items = resDto;

            return response;

        }

        public async Task<PagedResults<TodayTrendingTagResponse>> GetTodayTrendingTags(TodayTrendingTagReq todayTrendingTagReq)
        {
            int.TryParse(_configuration["TodayTrending:FetchDataTimes"], out var getdataTimes);
            int.TryParse(_configuration["TodayTrending:Days"], out var days);
            var offset = todayTrendingTagReq.PageSize * (todayTrendingTagReq.PageNumber - 1);
            var toDate = DateTime.UtcNow.Date.EndOfDay();
            var fromDate = toDate.AddDays(-days).BeginOfDay();

            IEnumerable<TodayTrendingTagResponse> resDto = null;
            PagedResults<TodayTrendingTagResponse> response = null;
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
                    response = new PagedResults<TodayTrendingTagResponse>(totalItems, todayTrendingTagReq.PageNumber, todayTrendingTagReq.PageSize);
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
                response = new PagedResults<TodayTrendingTagResponse>(0, todayTrendingTagReq.PageNumber, todayTrendingTagReq.PageSize);
            }
            return response;

        }
        public async Task<List<TagView>> GetTagsByPostIdAsync(Guid postId)
        {
            try
            {
                // Find all tag with post (TagPost)
                var query = string.Format(GetAllTagsByPostQuery, _tagRepository.TableName);
                var tagsPostDb = await _tagRepository
                        .Connection.QueryAsync<TagView>(query, new
                        {
                            PostId = postId
                        });

                return tagsPostDb.ToList();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }

        public async Task<List<TagByPostResponse>> GetTagsByPostHashIdAsync(string postHashId)
        {
            try
            {
                // Find all tag with post (TagPost)
                var tagsPostDb = await _tagRepository
                        .Connection.QueryAsync<TagByPostResponse>(GetAllTagsByPostHashIdQuery, new
                        {
                            PostHashId = postHashId
                        });

                return tagsPostDb.ToList();
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }

        private async Task<List<Guid>> AddNewTags(List<string> tagNames)
        {
            var userId = _currentUserService.Session.UserId;
            var tagsToInsert = new List<Tag>();
            var smartLookupInserts = new List<SmartLookup>();
            foreach (var tagName in tagNames)
            {
                tagsToInsert.Add(new Tag
                {
                    AuthorId = userId,
                    CreatedBy = userId,
                    LastModifiedBy = userId,
                    Name = tagName.Trim(),
                    Title = tagName,
                    IsDelete = false,
                });
                smartLookupInserts.Add(new SmartLookup
                {
                    CountCriteria = 0,
                    Keyword = tagName,
                    KeywordType = LookupKeywordType.Tag
                });
            }
            await _tagRepository.InsertAsync(tagsToInsert);
            await _smartLookupRepository.InsertAsync(smartLookupInserts);

            return tagsToInsert.Select(x => x.Id).ToList();

        }

        private async Task<IEnumerable<Guid>> AddTagsToPost(Guid postId, List<Guid> tagIds)
        {
            var userId = _currentUserService.Session.UserId;
            var tagPostsToInsert = new List<TagPost>();
            foreach (var tagId in tagIds)
            {
                tagPostsToInsert.Add(new TagPost
                {
                    IsDelete = false,
                    PostId = postId,
                    TagId = tagId,
                    CreatedBy = userId,
                    LastModifiedBy = userId
                });
            }
            await _tagPostRepository.InsertAsync(tagPostsToInsert);
            return tagPostsToInsert.Select(x => x.Id).ToList();
        }

        private string ValidateTag(string tag)
        {
            tag = (tag + "").TrimStart('#');

            if (tag.Length > SystemConfig.PostTagMaxLength)
            {
                throw new ArgumentException(ErrorCodes.PortalTagNameNotValidLength, string.Format(ErrorMessage.TagNameNotValidLength, tag));
            }

            // Hash tag without any special character, except underscore, number and character
            if (!Regex.IsMatch(tag, @"^[a-zA-Z0-9_]+$"))
            {
                throw new ArgumentException(ErrorCodes.PortalTagNameNotValid, string.Format(ErrorMessage.TagNameNotValid, tag));
            }

            return tag.ToLower();
        }

        public async Task<PagedResults<TagSearchResponse>> SearchTagbyKeyword(SearchTagReq input)
        {
            PagedResults<TagSearchResponse> results;
            if (string.IsNullOrWhiteSpace(input.Name))
            {
                return new PagedResults<TagSearchResponse>(0);
            }
            var keywords = input.Name.ToLower().Split(' ');
            var query = $@"
                                SELECT t.""Name"",COUNT( t.""Id"")   from ""Tags"" t 
                                LEFT JOIN ""TagPosts"" tp  
                                ON tp.""TagId""  = t.""Id"" 
                                LEFT JOIN ""Posts"" p 
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
                                    FROM ""Posts"" post
                                    INNER JOIN ""TagPosts"" tagpost ON post.""Id"" = tagpost.""PostId"" 
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
                results = new PagedResults<TagSearchResponse>(totalItems, input.PageNumber, input.PageSize);
                results.Items = items;
            }
            else
            {
                results = new PagedResults<TagSearchResponse>(0);
            }

            return results;
        }

        public async Task<IEnumerable<TagSearchResponse>> SearchTagsByName(SearchTagsReq request)
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
    }
}
