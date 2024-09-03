using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Comic.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain;
using Common.Domain.Entities;
using Common.SeedWork.Extensions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Requests;

public partial class SoundService : ISoundService
{
    public SoundService(IMcsgContext context, ISetting setting, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _setting = setting;
        _bgMediaRepository = unitOfWork.GetRepository<BackgroundMedia>();
        _bgMediaPostRepository = unitOfWork.GetRepository<BackgroundMediaPost>();
        _mapper = mapper;
    }

    public async Task<PagedResponse<BackgroundMedia.SearchDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req)
    {
        var offset = req.PageSize * (req.PageNumber - 1);
        var multi = await _bgMediaRepository.Connection.QueryMultipleAsync(GetAllSoundQuery,
                                                                        new
                                                                        {
                                                                            req.PageSize,
                                                                            Offet = offset
                                                                        });

        var items = await multi.ReadAsync<BackgroundMedia>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            var response = new PagedResponse<BackgroundMedia.SearchDto>(totalItems, req.PageNumber, req.PageSize);
            response.Items = items.Select(p => p.ToSearchDto(_setting.Api.Web.Media));
            return response;
        }
        else
        {
            return new PagedResponse<BackgroundMedia.SearchDto>(0);
        }
    }
    public async Task<PagedResponse<SoundRecentlyDto>> GetRecentlyUseSoundAsync(SoundBackgroundMediaLoadR req)
    {
        var offset = req.PageSize * (req.PageNumber - 1);
        var multi = await _bgMediaRepository.Connection.QueryMultipleAsync(GetRecentlyUseSoundQuery,
                                                                        new
                                                                        {
                                                                            req.PageSize,
                                                                            Offet = offset
                                                                        });

        var items = await multi.ReadAsync<SoundRecentlyDto>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            foreach (var item in items)
            {
                item.Duration = item.DurationSeconds.ToDuration();
                item.Url = _setting.Api.Web.Media.GetMediaPath(".mp3", item.Url, null);
                item.Thumbnail = _setting.Api.Web.Media.GetMediaPath(".jpg", item.Thumbnail, null);
            }

            var response = new PagedResponse<SoundRecentlyDto>(totalItems, req.PageNumber, req.PageSize);
            response.Items = items;

            return response;
        }
        else
        {
            return new PagedResponse<SoundRecentlyDto>(0);
        }
    }

    public async Task<PagedResponse<BackgroundMedia.SearchDto>> SearchSoundAsync(SoundSearchSoundR req)
    {
        if (string.IsNullOrWhiteSpace(req.Keyword))
        {
            return new PagedResponse<BackgroundMedia.SearchDto>(0);
        }

        req.Keyword = "%" + req.Keyword + "%";
        var offset = req.PageSize * (req.PageNumber - 1);
        var multi = await _bgMediaRepository.Connection.QueryMultipleAsync(SearchSoundByTitleQuery,
                                                                        new
                                                                        {
                                                                            req.PageSize,
                                                                            Offet = offset,
                                                                            req.Keyword
                                                                        });

        var items = await multi.ReadAsync<BackgroundMedia>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            var response = new PagedResponse<BackgroundMedia.SearchDto>(totalItems, req.PageNumber, req.PageSize);
            response.Items = items.Select(p => p.ToSearchDto(_setting.Api.Web.Media));
            return response;
        }
        else
        {
            return new PagedResponse<BackgroundMedia.SearchDto>(0);
        }
    }
    public async Task<BackgroundMedia.SearchDto> GetSoundByPostAsync(Guid postId)
    {
        var result = await _bgMediaRepository.Connection
                                                .QueryFirstOrDefaultAsync<BackgroundMedia>
                                                                (GetSoundByPostQuery, new { PostId = postId });
        if (result != null)
        {
            return _mapper.Map<BackgroundMedia.SearchDto>(result);
        }
        else
        {
            return null;
        }
    }

    public async Task<bool> AddSoundAsync(Guid postId, Guid soundId, Guid userId)
    {
        await RemoveSoundAsync(postId);

        var bgSoundPost = new BackgroundMediaPost
        {
            BackgroundMediaId = soundId,
            PostId = postId,
            Status = BackgroundMediaPostStatus.Add,
            CreatedBy = userId
        };

        await _context.BackgroundMediaPosts.AddAsync(bgSoundPost);
        var result = await _context.SaveChangesAsync(default);

        return result > 0;
    }

    public async Task<bool> RemoveSoundAsync(Guid postId)
    {
        var backgroundMediaPosts = await _context.BackgroundMediaPostAvailable.Where(p => p.PostId == postId).ToListAsync();
        backgroundMediaPosts.ForEach(p => p.Status = BackgroundMediaPostStatus.Remove);
        var result = await _context.SaveChangesAsync(default);

        return result > 0;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly IMcsgContext _context;

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    private readonly IRepository<BackgroundMedia> _bgMediaRepository;
    private readonly IRepository<BackgroundMediaPost> _bgMediaPostRepository;
    private readonly IMapper _mapper;

    #endregion
}
