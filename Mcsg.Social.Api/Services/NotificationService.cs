using AutoMapper;
using Dapper;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services;

using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain.Entities;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Lib.Common.Constants;
using Lib.Common.Web.Security;
using Lib.Data.Repositories;
using Lib.Data.Repositories.Interface;
using Models;
using Requests;
using static Common.Core.Constants.Setting;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class NotificationService : INotificationService
{
    private readonly ICurrentUserService _currentUserService;
    private readonly IRepository<Notification> _notiRepository;
    private readonly IRepository<NotificationObject> _notiObjRepository;
    private readonly IRepository<SocialPost> _postRepository;
    private readonly IRepository<User> _userRepository;

    private readonly IRepository<SocialPostReaction> _postReacRepository;
    private readonly IRepository<SocialSubPostReaction> _subPostReacRepository;
    private readonly IRepository<SocialPostCommentReaction> _postCommentRepository;
    private readonly IRepository<SocialSubPostCommentReaction> _subPostCommentRepository;
    private readonly IMapper _mapper;
    private IConfiguration _configuration;

    public NotificationService(ICurrentUserService currentUserService
        , IUnitOfWork unitOfWork
        , IMapper mapper
        , IConfiguration configuration
        , IRepository<SocialPostReaction> postReacRepository
        , IRepository<SocialSubPostReaction> subPostReacRepository
        , IRepository<SocialPostCommentReaction> postCommentRepository
        , ISetting setting
        , IRepository<SocialSubPostCommentReaction> subPostCommentRepository)
    {
        _currentUserService = currentUserService;
        _notiRepository = unitOfWork.GetRepository<Notification>();
        _notiObjRepository = unitOfWork.GetRepository<NotificationObject>();
        _userRepository = unitOfWork.GetRepository<User>();
        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _mapper = mapper;
        _configuration = configuration;
        _postReacRepository = postReacRepository;
        _subPostReacRepository = subPostReacRepository;
        _postCommentRepository = postCommentRepository;
        _setting = setting;
        _subPostCommentRepository = subPostCommentRepository;
    }

    public async Task<NotificationModel> GetNotificationAsync(Guid id)
    {
        var result = await _notiRepository.Connection.QueryFirstOrDefaultAsync<NotificationQueryResult>(GetNotificationByIdQuery, new { Id = id });
        return _mapper.Map<NotificationModel>(result);
    }

    public async Task<PagedResponse<NotificationModel>> GetNotificationByReceiverAsync(NotificationR request)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
        {
            throw new NotFoundException(E303, M303);
        }
        var receiverId = currentUser.UserId;
        var offset = request.PageSize * (request.PageNumber - 1);
        var multi = await _notiRepository.Connection.QueryMultipleAsync(GetNotificationByUserQuery,
                                                                        new
                                                                        {
                                                                            ReceiverId = receiverId,
                                                                            PageSize = request.PageSize,
                                                                            Offet = offset
                                                                        });

        var items = await multi.ReadAsync<NotificationQueryResult>().ConfigureAwait(false);

        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var resDto = _mapper.Map<List<NotificationModel>>(items);
            await UpdateComicStory(resDto);
            var response = new PagedResponse<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {
            return new PagedResponse<NotificationModel>(0);
        }
    }

    private async Task UpdateComicStory(List<NotificationModel> resDto)
    {
        var subComicIds = resDto.Where(p => p.TargetType == NotificationTargetType.CommentOnSubComic).Select(p => p.LocationId).ToList();
        if (subComicIds.Any())
        {
            var subComics = await _notiRepository.Connection.QueryAsync<SubPostData>($@"
                                        SELECT csp.""Id"",csp.""Order"",cp.""HashId""  from comic.""ComicSubPosts"" csp
                                        JOIN comic.""ComicPosts"" cp on csp.""PostId"" = cp.""Id""
                                        WHERE csp.""Id"" = ANY(@ids)", new { ids = subComicIds });
            if (subComics.Count() > 0)
            {
                foreach (var item in subComics)
                {
                    var comicResponse = resDto.FirstOrDefault(p => p.LocationId == item.Id);
                    if (comicResponse != null)
                    {
                        comicResponse.Order = item.Order;
                        comicResponse.LocationHashId = item.HashId;
                    }
                }
            }
        }

        var subStoryIds = resDto.Where(p => p.TargetType == NotificationTargetType.CommentOnSubStory).Select(p => p.LocationId).ToList();
        if (subStoryIds.Any())
        {
            var subStories = await _notiRepository.Connection.QueryAsync<SubPostData>($@"
                                        SELECT csp.""Id"",csp.""Order"",cp.""HashId""  from story.""StorySubPosts"" csp
                                        JOIN story.""StoryPosts"" cp on csp.""PostId"" = cp.""Id""
                                        WHERE csp.""Id"" = ANY(@SubPostIds)", new { SubPostIds = subStoryIds });
            if (subStories.Count() > 0)
            {
                foreach (var item in subStories)
                {
                    var storyResponse = resDto.FirstOrDefault(p => p.LocationId == item.Id);
                    if (storyResponse != null)
                    {
                        storyResponse.Order = item.Order;
                        storyResponse.LocationHashId = item.HashId;
                    }
                }
            }
        }
    }

    public async Task<PagedResponse<NotificationModel>> GetUnReadNotificationByReceiverAsync(NotificationR request)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
        {
            throw new NotFoundException(E303, M303);
        }
        var receiverId = currentUser.UserId;
        var offset = request.PageSize * (request.PageNumber - 1);
        var multi = await _notiRepository.Connection.QueryMultipleAsync(GetNotificationUnReadByUserQuery,
                                                                        new
                                                                        {
                                                                            ReceiverId = receiverId,
                                                                            PageSize = request.PageSize,
                                                                            Offet = offset
                                                                        });

        var items = await multi.ReadAsync<NotificationQueryResult>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            var resDto = _mapper.Map<List<NotificationModel>>(items);
            await UpdateComicStory(resDto);
            var response = new PagedResponse<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {

            return new PagedResponse<NotificationModel>(0);
        }
    }

    public async Task<bool> ReadAllNotificationAsync()
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
        {
            throw new NotFoundException(E303, M303);
        }


        var receiverId = currentUser.UserId;
        var result = await _notiRepository.Connection.ExecuteAsync(UpdateNotificationStatusQuery, new
        {
            Status = NotificationStatus.Read,
            ReceiverId = receiverId,
        });

        return result > 0;
    }

    public async Task<bool> ReadNotificationAsync(Guid id)
    {
        var currentUser = await _currentUserService.GetCurrentUserAsync();
        if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
        {
            throw new NotFoundException(E303, M303);
        }

        var notification = await _notiRepository.GetByIdAsync(id);
        if (notification == null)
        {
            throw new NotFoundException(ErrorCodes.QueryEmpty, ErrorCodes.QueryEmpty);
        }

        notification.Status = NotificationStatus.Read;
        notification.ModifiedOn = DateTime.UtcNow;
        notification.ModifiedBy = currentUser.UserId;

        return await _notiRepository.UpdateAsync(notification);
    }

    public async Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/reaction");

        var url = urlBuilder.ToString();

        var response = await url.MakePostRequest(req);

        if (response.IsSuccessStatusCode)
        {
            string responseContent = await response.Content.ReadAsStringAsync();
            var responseBody = JsonConvert.DeserializeObject<ApiNotificationDto>(responseContent);

            return true;
        }
        else
        {
            return false;
        }
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
