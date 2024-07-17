using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Distributor;
using Common.Core.Dtos;
using Common.Core.Enums;
using Common.Core.Extensions;
using Dtos;
using Interfaces;
using Lib.Data;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Models;
using Requests;

public partial class SmartLookupService : ISmartLookupService
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
    public SmartLookupService(McsgDbContext context, ISetting setting, DistributeManager distributeManager, IRepository<Tag> tagRepository, IRepository<SmartLookupUser> smartLookupUserRepository, IRepository<SmartLookup> smartLookupRepository, IRepository<TagPost> tagPostRepository, IRepository<User> userRepository)
    {
        _context = context;
        _setting = setting;
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

            if (result != null && result.Count() > 0)
            {
                foreach (var item in result)
                {
                    item.Avatar = _setting.Minio.MediaApiUrl.ToPublicImageUrl(item.Avatar);
                }
            }
        }

        return result;
    }

    public async Task<IEnumerable<RecentSearchResponse>> GetRecentListAsync(Guid userId)
    {
        var tag = nameof(LookupKeywordType.Tag);
        var people = nameof(LookupKeywordType.People);
        var q = _context.SmartLookupUserAvailable.Where(p => p.UserId == userId).OrderByDescending(x => x.CreatedDate).Take(6)
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
        return await _smartLookupUserRepository.DeleteAsync(id);
    }

    public async Task<bool> AddRecentSearchAsync(SmartLookupAddRecentSearchR res, Guid userId)
    {
        var smartLookupObj = new SmartLookupUser()
        {
            Id = Guid.NewGuid(),
            Keyword = res.Keyword,
            UserId = userId,
            CreatedDate = DateTime.UtcNow
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
            await _smartLookupUserRepository.DeleteAsync(existKeyword.Id);
        }

        await _smartLookupUserRepository.InsertAsync(smartLookupObj);

        return true;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly McsgDbContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    /// <summary>
    /// Distribute manager
    /// </summary>
    private readonly DistributeManager _distributeManager;

    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<SmartLookupUser> _smartLookupUserRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;

    private readonly IRepository<TagPost> _tagPostRepository;
    private readonly IRepository<User> _userRepository;

    #endregion
}
