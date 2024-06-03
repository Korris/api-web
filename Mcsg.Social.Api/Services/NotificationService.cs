using AutoMapper;
using Dapper;
using Mcsg.Api.DTOs;
using Mcsg.Api.Models;
using Mcsg.Lib.Common.Constants;
using Mcsg.Lib.Common.Exceptions;
using Mcsg.Lib.Common.Helpers;
using Mcsg.Lib.Common.Web.Security;
using Mcsg.Lib.Data.Domain.Entities;
using Mcsg.Lib.Data.Entities.Common;
using Mcsg.Lib.Data.Enums;
using Mcsg.Lib.Data.Repositories;
using Mcsg.Lib.Data.Repositories.Interface;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Mcsg.Api.Services
{
    public interface INotificationService
    {
        Task<NotificationModel> GetNotificationAsync(Guid id);
        Task<bool> ReadNotificationAsync(Guid id);
        Task<bool> ReadAllNotificationAsync();
        Task<PagedResults<NotificationModel>> GetNotificationByReceiverAsync(NotificationReq request);
        Task<PagedResults<NotificationModel>> GetUnReadNotificationByReceiverAsync(NotificationReq request);
        Task<bool> AddVideoNotificationAsync(VideoNotificationReq req);
        Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req);
    }
    public partial class NotificationService : INotificationService
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRepository<Notification> _notiRepository;
        private readonly IRepository<NotificationObject> _notiObjRepository;
        private readonly IRepository<Post> _postRepository;
        private readonly IRepository<User> _userRepository;

        private readonly IRepository<PostReaction> _postReacRepository;
        private readonly IRepository<SubPostReaction> _subPostReacRepository;
        private readonly IRepository<PostCommentReaction> _postCommentRepository;
        private readonly IRepository<SubPostCommentReaction> _subPostCommentRepository;
        private readonly IMapper _mapper;
        private readonly RealTimeServiceSetting _realTimeServiceSetting;
        private IConfiguration _configuration;
        public NotificationService(ICurrentUserService currentUserService
            , IUnitOfWork unitOfWork
            , IMapper mapper
            , IConfiguration configuration
            , IOptionsMonitor<RealTimeServiceSetting> realTimeServiceSetting
            , IRepository<PostReaction> postReacRepository
            , IRepository<SubPostReaction> subPostReacRepository
            , IRepository<PostCommentReaction> postCommentRepository
            , IRepository<SubPostCommentReaction> subPostCommentRepository)
        {
            _currentUserService = currentUserService;
            _notiRepository = unitOfWork.GetRepository<Notification>();
            _notiObjRepository = unitOfWork.GetRepository<NotificationObject>();
            _userRepository = unitOfWork.GetRepository<User>();
            _postRepository = unitOfWork.GetRepository<Post>();
            _mapper = mapper;
            _realTimeServiceSetting = realTimeServiceSetting.CurrentValue;
            _configuration = configuration;
            _postReacRepository = postReacRepository;
            _subPostReacRepository = subPostReacRepository;
            _postCommentRepository = postCommentRepository;
            _subPostCommentRepository = subPostCommentRepository;
        }

        public async Task<NotificationModel> GetNotificationAsync(Guid id)
        {
            var result = await _notiRepository.Connection.QueryFirstOrDefaultAsync<NotificationQueryResult>(GetNotificationByIdQuery, new { Id = id });
            return _mapper.Map<NotificationModel>(result);
        }

        public async Task<PagedResults<NotificationModel>> GetNotificationByReceiverAsync(NotificationReq request)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
            {
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
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
                foreach (var item in resDto)
                {
                    item.Avatar = UrlHelper.GetPublicImageUrl(_configuration, item.Avatar);
                }
                var response = new PagedResults<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
                response.Items = resDto;

                return response;
            }
            else
            {
                return new PagedResults<NotificationModel>(0);
            }
        }

        public async Task<PagedResults<NotificationModel>> GetUnReadNotificationByReceiverAsync(NotificationReq request)
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
            {
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
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
                foreach (var item in resDto)
                {
                    item.Avatar = UrlHelper.GetPublicImageUrl(_configuration, item.Avatar);
                }
                var response = new PagedResults<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
                response.Items = resDto;

                return response;
            }
            else
            {

                return new PagedResults<NotificationModel>(0);
            }
        }

        public async Task<bool> ReadAllNotificationAsync()
        {
            var currentUser = await _currentUserService.GetCurrentUserAsync();
            if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.SessionId))
            {
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
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
                throw new NotFoundException(ErrorCodes.NotExistedUser, ErrorMessage.AccountNotExist);
            }

            var notification = await _notiRepository.GetByIdAsync(id);
            if (notification == null)
            {
                throw new NotFoundException(ErrorCodes.QueryEmpty, ErrorCodes.QueryEmpty);
            }

            notification.Status = Lib.Data.Enums.NotificationStatus.Read;
            notification.LastModifiedDate = DateTime.UtcNow;
            notification.LastModifiedBy = currentUser.UserId;

            return await _notiRepository.UpdateAsync(notification);
        }

        public async Task<bool> AddVideoNotificationAsync(VideoNotificationReq req)
        {
            var baseUrl = _realTimeServiceSetting.BaseUrl;
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/video");

            var url = urlBuilder.ToString();

            var response = await HttpHelper.MakePostRequest(url, req);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var responseBody = JsonConvert.DeserializeObject<ApiResponseOfNotification>(responseContent);

                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> AddReactionNotificationAsync(ReactionNotificationReq req)
        {
            var baseUrl = _realTimeServiceSetting.BaseUrl;
            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/reaction");

            var url = urlBuilder.ToString();

            var response = await HttpHelper.MakePostRequest(url, req);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                var responseBody = JsonConvert.DeserializeObject<ApiResponseOfNotification>(responseContent);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
