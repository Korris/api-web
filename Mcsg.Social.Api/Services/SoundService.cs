using AutoMapper;
using Dapper;
using Microsoft.EntityFrameworkCore;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.SeedWork.Responses;
using Dtos;
using Extensions;
using Interfaces;
using Lib.Data;
using Lib.Data.Domain.Entities;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Requests;

public partial class SoundService : ISoundService
{
    public SoundService(McsgDbContext context, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _context = context;
        _bgMediaRepository = unitOfWork.GetRepository<BackgroundMedia>();
        _bgMediaPostRepository = unitOfWork.GetRepository<BackgroundMediaPost>();
        _mapper = mapper;
    }

    public async Task<PagedResponse<SoundDto>> GetAllSoundAsync(SoundBackgroundMediaLoadR req)
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

            var resDto = _mapper.Map<IEnumerable<SoundDto>>(items);
            var response = new PagedResponse<SoundDto>(totalItems, req.PageNumber, req.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {
            return new PagedResponse<SoundDto>(0);
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
                item.Url = item.Url.ToAudioPath();
                item.Thumbnail = item.Thumbnail.ToImagePath();
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

    public async Task<PagedResponse<SoundDto>> SearchSoundAsync(SoundSearchSoundR req)
    {
        if (string.IsNullOrWhiteSpace(req.Keyword))
        {
            return new PagedResponse<SoundDto>(0);
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

            var resDto = _mapper.Map<IEnumerable<SoundDto>>(items);
            var response = new PagedResponse<SoundDto>(totalItems, req.PageNumber, req.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {
            return new PagedResponse<SoundDto>(0);
        }
    }
    public async Task<SoundDto> GetSoundByPostAsync(Guid postId)
    {
        var result = await _bgMediaRepository.Connection
                                                .QueryFirstOrDefaultAsync<BackgroundMedia>
                                                                (GetSoundByPostQuery, new { PostId = postId });
        if (result != null)
        {
            return _mapper.Map<SoundDto>(result);
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
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    public async Task<bool> RemoveSoundAsync(Guid postId)
    {
        var backgroundMediaPosts = await _context.BackgroundMediaPostAvailable.Where(p => p.PostId == postId).ToListAsync();
        backgroundMediaPosts.ForEach(p => p.Status = BackgroundMediaPostStatus.Remove);
        var result = await _context.SaveChangesAsync();

        return result > 0;
    }

    #region -- Fields --

    /// <summary>
    /// DB context
    /// </summary>
    private readonly McsgDbContext _context;

    private readonly IRepository<BackgroundMedia> _bgMediaRepository;
    private readonly IRepository<BackgroundMediaPost> _bgMediaPostRepository;
    private readonly IMapper _mapper;

    #endregion
}
