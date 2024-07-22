using Dapper;

namespace Mcsg.Admin.Api.Services
{
    using Common.Core.Constants;
    using Common.Core.Enums;
    using Common.Domain.Entities;
    using Common.SeedWork.Exceptions;
    using Common.SeedWork.Responses;
    using Dtos;
    using Interface;
    using Lib.Common.Constants;
    using Lib.Common.Interfaces;
    using Lib.Common.Web.Security;
    using Lib.Data.Constants;
    using Lib.Data.Repositories;
    using Requests;

    public partial class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepo;
        private readonly ILogger<PostService> _logger;
        private readonly IValidator<CreateAdminReq> _createAdminValidator;
        private readonly ICurrentUserService _currentUserService;

        public PostService(IRepository<Post> postRepo
            , ILogger<PostService> logger
            , IValidator<CreateAdminReq> createAdminValidator
            , ICurrentUserService currentUserService)
        {
            _postRepo = postRepo;
            _logger = logger;
            _createAdminValidator = createAdminValidator;
            _currentUserService = currentUserService;
        }

        public async Task<PagedResponse<PostBasicResponse>> GetListAsync(PostListReq request)
        {
            try
            {
                PagedResponse<PostBasicResponse> results;
                var offset = (request.PageNumber - 1) >= 0 ? request.PageSize * (request.PageNumber - 1) : 0;

                var strOrderBy = request.OrderByAsc ? SQLConstant.OrderByDesc : "";

                //TODO: this columns don't exist in database. We will remove in feature when we implement and add this columns.
                var ignoreOderByColumn = new List<string>{

                                            $"{nameof(PostBasicResponse.HashId)}",
                                            $"{nameof(PostBasicResponse.UserId)}"};

                if (request.OrderBy == null || ignoreOderByColumn.Contains(request.OrderBy, StringComparer.OrdinalIgnoreCase))
                {
                    request.OrderBy = nameof(Post.CreatedDate);
                }
                else
                {
                    request.OrderBy = $"{char.ToUpper(request.OrderBy[0])}{request.OrderBy.Substring(1)}";
                }

                if (AliasAndAmbiguousColumns.TryGetValue(request.OrderBy, out string columnName))
                {
                    request.OrderBy = columnName;
                }
                else
                {
                    request.OrderBy = $"\"{request.OrderBy}\"";
                }

                var query = string.Format(GetAllPostsQuery, _postRepo.TableName, DbSchema.Default, request.OrderBy, strOrderBy);


                if (!string.IsNullOrEmpty(request.HashId))
                {
                    query = query.Replace("[HashId]", @"p.""HashId"" = @HashId AND (@NotSelectDeleted = true OR p.""IsDelete"" = false) ");
                }
                else
                {
                    query = query.Replace("[HashId]", @"(@NotSelectDeleted = true OR p.""IsDelete"" = false) ");
                }

                if (!string.IsNullOrEmpty(request.TitleSearch))
                {
                    query = query.Replace("[TitleSearch]", string.Format(@"AND p.""Title"" ILIKE '%{0}%' ", request.TitleSearch));
                }
                else
                {
                    query = query.Replace("[TitleSearch]", @"");
                }

                if (request.FromDate != null)
                {
                    query = query.Replace("[FromDate]", @"AND p.""CreatedDate"" >= @FromDate");
                }
                else
                {
                    query = query.Replace("[FromDate]", @"");
                }
                if (request.ToDate != null)
                {
                    query = query.Replace("[ToDate]", @"AND p.""CreatedDate"" <= @ToDate");
                }
                else
                {
                    query = query.Replace("[ToDate]", @"");
                }
                if (request.PostType != null)
                {
                    query = query.Replace("[PostType]", @"AND p.""Type"" = @PostType");
                }
                else
                {
                    query = query.Replace("[PostType]", @"");
                }

                var multipleQuery = await _postRepo.Connection.QueryMultipleAsync(query, new
                {
                    NotSelectDeleted = true,
                    PageSize = request.PageSize,
                    Offet = offset,
                    HashId = request.HashId,
                    FromDate = request.FromDate,
                    ToDate = request.ToDate,
                    PostType = (int?)request.PostType
                });
                var items = await multipleQuery.ReadAsync<PostBasicResponse>().ConfigureAwait(false);

                var totalItems = await multipleQuery.ReadFirstAsync<int>().ConfigureAwait(false);

                if (items != null && items.Any())
                {
                    foreach (var item in items)
                    {
                        if (item.Tags != null && item.Tags.Length > 1)
                            item.Tags = item.Tags.Distinct().ToArray();
                    }

                    results = new PagedResponse<PostBasicResponse>(totalItems, request.PageNumber, request.PageSize)
                    {
                        Items = items
                    };
                }
                else
                {
                    results = new PagedResponse<PostBasicResponse>(0);
                }
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetListAsync), request);
                throw new BadRequestException(ErrorCodes.QuerySyntaxWrong, ex.Message);
            }
        }
        public async Task<bool> DeactivePostAsync(Guid id, PostStatusReq request)
        {
            var isResult = true;
            var post = await _postRepo.GetByIdAsync(id);
            if (post == null)
            {
                return isResult; // do nothing
            }
            if (post.Status == PostStatus.Public)
                post.Status = PostStatus.Inactive;
            post.StatusReason = request.Reason;
            post.ModifiedBy = _currentUserService?.Session?.UserId;
            post.ModifiedDate = DateTime.UtcNow;

            return await _postRepo.UpdateAsync(post);
        }
        public async Task<bool> DeletePostAsync(Guid id, PostStatusReq request)
        {
            var isResult = false;
            var post = await _postRepo.GetByIdAsync(id, "\"Id\"");
            if (post == null)
            {
                return isResult; // do nothing
            }
            isResult = true;
            post.StatusReason = request.Reason;
            post.ModifiedBy = _currentUserService?.Session?.UserId;
            post.ModifiedDate = DateTime.UtcNow;

            return await _postRepo.UpdateAsync(post);
        }
    }
}
