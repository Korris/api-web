using AutoMapper;
using Dapper;
using Mcsg.Api.DTOs;
using Mcsg.Api.Extensions;
using Mcsg.Api.Services.Interfaces;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;

namespace Mcsg.Api.Services
{
    public partial class SoundService : ISoundService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<BackgroundMedia> _bgMediaRepository;
        private readonly IRepository<BackgroundMediaPost> _bgMediaPostRepository;
        private readonly IMapper _mapper;
        public SoundService(ICurrentUserService currentUserService
            , IUnitOfWork unitOfWork
            , IMapper mapper)
        {
            _currentUserService = currentUserService;
            _postRepository = unitOfWork.GetRepository<Post>();
            _bgMediaRepository = unitOfWork.GetRepository<BackgroundMedia>();
            _bgMediaPostRepository = unitOfWork.GetRepository<BackgroundMediaPost>();
            _mapper = mapper;
        }

        public async Task<PagedResults<SoundDto>> GetAllSoundAsync(BackgroundMediaLoadReq req)
        {
            var offset = req.PageSize * (req.PageNumber - 1);
            var multi = await _bgMediaRepository.Connection.QueryMultipleAsync(GetAllSoundQuery,
                                                                            new
                                                                            {
                                                                                PageSize = req.PageSize,
                                                                                Offet = offset
                                                                            });

            var items = await multi.ReadAsync<BackgroundMedia>().ConfigureAwait(false);
            if (items != null)
            {
                var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

                var resDto = _mapper.Map<IEnumerable<SoundDto>>(items);
                var response = new PagedResults<SoundDto>(totalItems, req.PageNumber, req.PageSize);
                response.Items = resDto;

                return response;
            }
            else
            {
                return new PagedResults<SoundDto>(0);
            }
        }
        public async Task<PagedResults<SoundRecentlyDto>> GetRecentlyUseSoundAsync(BackgroundMediaLoadReq req)
        {
            var offset = req.PageSize * (req.PageNumber - 1);
            var multi = await _bgMediaRepository.Connection.QueryMultipleAsync(GetRecentlyUseSoundQuery,
                                                                            new
                                                                            {
                                                                                PageSize = req.PageSize,
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
                var response = new PagedResults<SoundRecentlyDto>(totalItems, req.PageNumber, req.PageSize);
                response.Items = items;

                return response;
            }
            else
            {
                return new PagedResults<SoundRecentlyDto>(0);
            }
        }

        public async Task<PagedResults<SoundDto>> SearchSoundAsync(SearchSoundReq req)
        {
            if (string.IsNullOrWhiteSpace(req.Keyword))
            {
                return new PagedResults<SoundDto>(0);
            }

            req.Keyword = "%" + req.Keyword + "%";
            var offset = req.PageSize * (req.PageNumber - 1);
            var multi = await _bgMediaRepository.Connection.QueryMultipleAsync(SearchSoundByTitleQuery,
                                                                            new
                                                                            {
                                                                                PageSize = req.PageSize,
                                                                                Offet = offset,
                                                                                Keyword = req.Keyword
                                                                            });

            var items = await multi.ReadAsync<BackgroundMedia>().ConfigureAwait(false);
            if (items != null)
            {
                var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

                var resDto = _mapper.Map<IEnumerable<SoundDto>>(items);
                var response = new PagedResults<SoundDto>(totalItems, req.PageNumber, req.PageSize);
                response.Items = resDto;

                return response;
            }
            else
            {
                return new PagedResults<SoundDto>(0);
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
        public async Task<bool> AddSoundAsync(Guid postId, Guid soundId)
        {
            await _bgMediaPostRepository.Connection.ExecuteAsync(RemoveAllSoundOfPostQuery, new { PostId = postId });
            var bgSoundPost = new BackgroundMediaPost()
            {
                BackgroundMediaId = soundId,
                PostId = postId,
                Status = BackgroundMediaPostStatus.Add,
                CreatedBy = _currentUserService.Session.UserId
            };

            var result = await _bgMediaPostRepository.InsertAsync(bgSoundPost);
            return result > 0;
        }

        public async Task<bool> RemoveSoundAsync(Guid postId)
        {
            var result = await _bgMediaPostRepository.Connection.ExecuteAsync(RemoveAllSoundOfPostQuery,
                                                    new { PostId = postId });
            return result > 0;
        }
    }
}
