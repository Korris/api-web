using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Api.Areas.Story.Services;
using Mcsg.Api.Interfaces;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.Interfaces;
using Mcsg.Api.Areas.Story.Dtos;
using Mcsg.Api.Areas.Story.Interfaces;
using Mcsg.Api.Areas.Story.Models;
using Mcsg.Api.Areas.Story.Requests;

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
    public SmartLookupService(IMcsgContext context, ISetting setting, DistributeManager distributeManager, IRepository<Tag> tagRepository, IRepository<SmartLookupUser> smartLookupUserRepository, IRepository<SmartLookup> smartLookupRepository, IRepository<StoryTagPost> tagPostRepository, IRepository<User> userRepository) : base(context, setting)
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
        var q = _context.SmartLookupUsers.Where(p => p.UserId == userId).OrderByDescending(x => x.CreatedOn).Take(6)
            .Select(p => new RecentSearchResponse
            {
                Id = p.Id,
                Keyword = p.Keyword + "",
                KeywordType = p.KeywordType == LookupKeywordType.Tag ? tag : p.KeywordType == LookupKeywordType.People ? people : string.Empty
            });

        return await q.ToListAsync();
    }

    public async Task<bool> DeleteRecentSearchAsync(Guid id)
    {
        return await _context.SmartLookupUsers.Where(p => p.Id == id).ExecuteDeleteAsync() > 0;
    }

    public async Task<bool> AddRecentSearchAsync(SmartLookupAddRecentSearchR res, Guid userId)
    {
        var smartLookupObj = new SmartLookupUser
        {
            Keyword = res.Keyword,
            UserId = userId,
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

        var existKeyword = (await _smartLookupUserRepository.Connection
            .QueryAsync<SmartLookupUser>(GetSmartLookupUserByKeywordAndTypeQuery, new
            {
                keyword = smartLookupObj.Keyword,
                type = smartLookupObj.KeywordType,
                userid = userId
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

    private readonly IRepository<StoryTagPost> _tagPostRepository;
    private readonly IRepository<User> _userRepository;

    #endregion
}
