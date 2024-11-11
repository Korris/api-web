using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Dtos;
using Interfaces;
using Models;
using Requests;

public partial class SmartLookupService : BaseSettingS, ISmartLookupService
{
    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="context"></param>
    /// <param name="setting"></param>
    /// <param name="distributeManager"></param>
    /// <param name="tagRepository"></param>
    /// <param name="smartLookupUserRepository"></param>
    /// <param name="smartLookupRepository"></param>
    /// <param name="tagPostRepository"></param>
    /// <param name="userRepository"></param>
    public SmartLookupService(IMcsgContext context, ISetting setting, DistributeManager distributeManager, IRepository<Tag> tagRepository, IRepository<SmartLookupUser> smartLookupUserRepository, IRepository<SmartLookup> smartLookupRepository, IRepository<SocialTagPost> tagPostRepository, IRepository<User> userRepository) : base(context, setting)
    {
        _distributeManager = distributeManager;

        _tagRepository = tagRepository;
        _smartLookupUserRepository = smartLookupUserRepository;
        _smartLookupRepository = smartLookupRepository;

        _tagPostRepository = tagPostRepository;
        _userRepository = userRepository;
    }

    public async Task CalculateSmartLookupWhenDeletePostAsync(Guid postId, string profileName)
    {
        // Calculate Smart lookup
        await _distributeManager.Deliver(new SmartLookupDistributeItem
        {
            Data = new SmartLookupDto
            {
                KeywordType = LookupKeywordType.People,
                ProfileName = profileName
            }
        });

        var tagNames = await _tagRepository.Connection.QueryAsync<string>(GetTagNamesByPostIdQuery, new { postId });
        if (tagNames != null && tagNames.Any())
        {
            await _distributeManager.Deliver(new SmartLookupDistributeItem
            {
                Data = new SmartLookupDto
                {
                    KeywordType = LookupKeywordType.Tag,
                    Tags = tagNames.ToList()
                }
            });
        }
    }

    public async Task CalculateSmartLookupWhenCreatePostAsync(string profileName)
    {
        //publish calculate smart lookup
        await _distributeManager.Deliver(new SmartLookupDistributeItem
        {
            Data = new SmartLookupDto
            {
                KeywordType = LookupKeywordType.People,
                ProfileName = profileName
            }
        });
    }

    public async Task CalculateSmartLookupForTagAsync(List<string> tags)
    {
        await _distributeManager.Deliver(new SmartLookupDistributeItem
        {
            Data = new SmartLookupDto
            {
                KeywordType = LookupKeywordType.Tag,
                Tags = tags
            }
        });
    }

    public async Task<IEnumerable<SmartLookupResponse>> SearchAsync(string keyword)
    {
        IEnumerable<SmartLookupResponse> result = null;
        if (keyword.Length > 0)
        {
            var decodedKeyWord = Uri.UnescapeDataString(keyword);
            string whereCondition;
            decodedKeyWord = decodedKeyWord.Replace("'", "''");

            if (decodedKeyWord[0] == '#')
            {
                whereCondition = @$"Where ""Keyword"" ILIKE '%{decodedKeyWord.Substring(1, decodedKeyWord.Length - 1)}%' AND ""KeywordType""={(int)LookupKeywordType.Tag}";
            }
            else
            {
                whereCondition = @$"Where ""Keyword"" ILIKE '%{decodedKeyWord}%'";
            }

            string query = string.Format(GetSmartLookupQuery, whereCondition);
            result = await _smartLookupRepository.Connection.QueryAsync<SmartLookupResponse>(query);
        }

        return result;
    }

    public async Task<IEnumerable<RecentSearchResponse>> GetRecentListAsync(Guid userId)
    {
        var tag = nameof(LookupKeywordType.Tag);
        var people = nameof(LookupKeywordType.People);
        var q = from slu in _context.SmartLookupUserAvailable
                join u in _context.UserAvailable on slu.Keyword equals u.ProfileName into userGroup
                from u in userGroup.DefaultIfEmpty()
                where slu.UserId == userId
                orderby slu.CreatedOn descending
                select new RecentSearchResponse
                {
                    Id = slu.Id,
                    Keyword = slu.Keyword + "",
                    KeywordType = slu.KeywordType == LookupKeywordType.Tag ? tag
                        : slu.KeywordType == LookupKeywordType.People ? people
                        : string.Empty,
                    EntityType = slu.EntityType.ToString(),
                    EntityId = slu.EntityId,
                    Avatar = u.Avatar ?? string.Empty
                };

        return await q.Take(6).ToListAsync();
    }

    public async Task<bool> DeleteRecentSearchAsync(Guid id)
    {
        return await _context.SmartLookupUsers.Where(p => p.Id == id).ExecuteDeleteAsync() > 0;
    }

    public async Task<bool> AddRecentSearchAsync(SmartLookupAddRecentSearchR res, Guid userId)
    {
        var entityType = EntityType.Post;
        var keywordType = Enum.Parse<LookupKeywordType>(res.KeywordType);

        switch (keywordType)
        {
            case LookupKeywordType.Tag:
                entityType = EntityType.Tag;
                break;
            case LookupKeywordType.Comic:
            case LookupKeywordType.Story:
                entityType = EntityType.Post;
                break;
            default:
                entityType = EntityType.User;
                break;
        }

        var smartLookupObj = new SmartLookupUser
        {
            Keyword = res.Keyword,
            UserId = userId,
            EntityType = entityType,
            CreatedOn = DateTime.UtcNow
        };

        if (string.IsNullOrEmpty(res.KeywordType))
        {
            smartLookupObj.KeywordType = LookupKeywordType.None;
        }
        else
        {
            smartLookupObj.KeywordType = Enum.Parse<LookupKeywordType>(res.KeywordType);
        }

        smartLookupObj.EntityId = entityType switch
        {
            EntityType.Post => _context.SocialPosts
                .Where(p => EF.Functions.ILike(p.Title, res.Keyword))
                .Select(p => p.Id).FirstOrDefault(),
            EntityType.User => _context.UserAvailable.Where(p => p.ProfileName == res.Keyword).Select(p => p.Id)
                .FirstOrDefault(),
            EntityType.Tag => _context.TagAvailable.Where(p => p.Name == res.Keyword).Select(p => p.Id)
                .FirstOrDefault(),
            _ => smartLookupObj.EntityId
        };

        var existKeyword = (await _smartLookupUserRepository.Connection
            .QueryAsync<SmartLookupUser>(GetSmartLookupUserByKeywordAndTypeQuery, new
            {
                keyword = smartLookupObj.Keyword,
                type = smartLookupObj.KeywordType,
                userid = userId,
                entityId = smartLookupObj.EntityId
            })).FirstOrDefault();

        if (existKeyword != null)
        {
            return await _context.SmartLookupUsers.Where(p => p.Id == existKeyword.Id).ExecuteDeleteAsync() > 0;
        }

        await _context.SmartLookupUsers.AddAsync(smartLookupObj);
        return await _context.SaveChangesAsync(default) > 0;
    }

    #region -- Fields --

    /// <summary>
    /// Distribute manager
    /// </summary>
    private readonly DistributeManager _distributeManager;

    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<SmartLookupUser> _smartLookupUserRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;

    private readonly IRepository<SocialTagPost> _tagPostRepository;
    private readonly IRepository<User> _userRepository;

    #endregion
}
