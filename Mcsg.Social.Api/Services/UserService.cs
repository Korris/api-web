using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Responses;
using Interfaces;
using Models;
using Requests;

public partial class UserService : IUserService
{
    public UserService(IMcsgContext context, IRepository<User> userRepository)
    {
        _context = context;
        _userRepository = userRepository;
    }

    public async Task<PagedResponse<UserSearchResponse>> SearchUserbyKeyword(SmartLookupSearchUserR input)
    {
        PagedResponse<UserSearchResponse> results;
        if (string.IsNullOrWhiteSpace(input.ProfileName))
        {
            return new PagedResponse<UserSearchResponse>(0);
        }
        var keywords = input.ProfileName.ToLower().Split(' ');
        var query = $@"SELECT ""ProfileName"",
                                  ""ProfileId"",
                                  ""Avatar"",
                                  ""UserName"",
                                  ""Id""
                          FROM identity.""Users"" 
                          [QueryCondition]
                          OFFSET @Offset 
                          LIMIT @PageSize;

                          SELECT COUNT(*) AS TotalItems 
                          FROM identity.""Users""
                          [QueryCondition]";
        bool first = true;
        var queryCondition = "";
        queryCondition += @"WHERE ""IsDelete"" = false AND (";
        foreach (var word in keywords)
        {
            if (first)
            {
                queryCondition += $@"LOWER(""ProfileName"") LIKE '%{word}%'";
                first = false;
            }
            else
            {
                queryCondition += $@"OR LOWER(""ProfileName"") LIKE '%{word}%'";
            }
        }
        queryCondition += ")";
        query = query.Replace("[QueryCondition]", queryCondition);
        var offset = input.PageSize * (input.PageNumber - 1);
        var multi = await _userRepository.Connection.QueryMultipleAsync(query, new
        {
            Offset = offset,
            PageSize = input.PageSize
        });
        var items = await multi.ReadAsync<UserSearchResponse>().ConfigureAwait(false);
        var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

        if (items.Any())
        {
            results = new PagedResponse<UserSearchResponse>(totalItems, input.PageNumber, input.PageSize);
            var userId = input.UserId;
            bool isHaveUser = false;
            var userFollowingIds = new List<Guid>();
            if (userId != null)
            {
                userFollowingIds = await _context.UserFollowAvailable.AsNoTracking()
                                                                        .Where(p => p.UserFollowerId == userId)
                                                                        .Select(p => p.UserFollowingId)
                                                                        .ToListAsync();
                isHaveUser = userFollowingIds.Count > 0;
            }

            if (isHaveUser)
            {
                foreach (var item in items)
                {
                    item.IsFollowing = userFollowingIds.Contains(item.Id);
                }
            }

            results.Items = items;
        }
        else
        {
            results = new PagedResponse<UserSearchResponse>(0);
        }
        return results;
    }

    #region -- Fields --

    /// <summary>
    /// DB Context
    /// </summary>
    private readonly IMcsgContext _context;

    private readonly IRepository<User> _userRepository;

    #endregion
}
