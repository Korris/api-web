using Dapper;

namespace Mcsg.Social.Api.Services;

using Interfaces;
using Lib.Common.Distributor;
using Lib.Common.Helpers;
using Lib.Common.Models;
using Lib.Common.Web.Security;
using Lib.Data.Domain.Entities;
using Lib.Data.Enums;
using Lib.Data.Repositories;
using Models;
using Requests;

public partial class SmartLookupService : ISmartLookupService
{
    private readonly DistributeManager _distributeManager;
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IRepository<TagPost> _tagPostRepository;
    private readonly IRepository<SmartLookupUser> _smartLookupUserRepository;
    private readonly IRepository<SmartLookup> _smartLookupRepository;
    private readonly IRepository<User> _userRepository;
    private IConfiguration _configuration;

    public SmartLookupService(DistributeManager distributeManager,
        ICurrentUserService currentUserService,
        IRepository<Tag> tagRepository,
        IRepository<TagPost> tagPostRepository,
        IRepository<SmartLookupUser> smartLookupUserRepository,
        IRepository<SmartLookup> smartLookupRepository,
        IRepository<User> userRepository,
        ISetting setting,
        IConfiguration configuration)
    {
        _distributeManager = distributeManager;
        _currentUserService = currentUserService;
        _tagRepository = tagRepository;
        _tagPostRepository = tagPostRepository;
        _smartLookupUserRepository = smartLookupUserRepository;
        _smartLookupRepository = smartLookupRepository;
        _userRepository = userRepository;
        _setting = setting;
        _configuration = configuration;
    }
    public async Task CalculateSmartLookupWhenDeletePostAsync(Guid postId)
    {
        // Calculate Smart lookup
        await _distributeManager.Deliver(new SmartLookupDistributeItem
        {
            Data = new SmartLookupData
            {
                KeywordType = LookupKeywordType.People,
                ProfileName = _currentUserService.Session.ProfileName
            }
        });

        var tagNames = await _tagRepository.Connection.QueryAsync<string>(GetTagNamesByPostIdQuery, new { postId });
        if (tagNames != null && tagNames.Any())
        {
            await _distributeManager.Deliver(new SmartLookupDistributeItem
            {
                Data = new SmartLookupData
                {
                    KeywordType = LookupKeywordType.Tag,
                    Tags = tagNames.ToList()
                }
            });
        }
    }

    public async Task CalculateSmartLookupWhenCreatePostAsync()
    {
        //publish calculate smart lookup
        await _distributeManager.Deliver(new SmartLookupDistributeItem
        {
            Data = new SmartLookupData
            {
                KeywordType = LookupKeywordType.People,
                ProfileName = _currentUserService.Session.ProfileName,
            }
        });
    }

    public async Task CalculateSmartLookupForTagAsync(List<string> tags)
    {
        await _distributeManager.Deliver(new SmartLookupDistributeItem
        {
            Data = new SmartLookupData
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
                    item.Avatar = _setting.Minio.MediaApiUrl.GetPublicImageUrl(item.Avatar);
                }
            }
        }

        return result;
    }

    public async Task<IEnumerable<RecentSearchResponse>> GetRecentListAsync()
    {
        var result = await _smartLookupUserRepository.Connection.QueryAsync<RecentSearchResponse>(GetRecentSearchQuery);
        return result;
    }

    public async Task<bool> DeleteRecentSearchAsync(Guid id)
    {
        return await _smartLookupUserRepository.DeleteAsync(id);
    }

    public async Task<bool> AddRecentSearchAsync(SmartLookupAddRecentSearchR res)
    {
        if (_currentUserService.Session != null && _currentUserService.Session.UserId != Guid.Empty)
        {
            var smartLookupObj = new SmartLookupUser()
            {
                Id = Guid.NewGuid(),
                Keyword = res.Keyword,
                UserId = _currentUserService.Session.UserId,
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
                    userid = _currentUserService.Session.UserId
                })).FirstOrDefault();

            if (existKeyword != null)
            {
                await _smartLookupUserRepository.DeleteAsync(existKeyword.Id);
            }

            await _smartLookupUserRepository.InsertAsync(smartLookupObj);
        }
        return true;
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
