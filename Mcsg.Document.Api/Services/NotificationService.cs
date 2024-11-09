using AutoMapper;
using Dapper;
using Newtonsoft.Json;

namespace Mcsg.Document.Api.Services;

using Common.Core.Constants;
using Common.Core.Enums;
using Common.Core.Extensions;
using Common.Domain.Entities;
using Common.Interfaces;
using Common.SeedWork.Exceptions;
using Common.SeedWork.Responses;
using Dtos;
using Interfaces;
using Models;
using Requests;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class NotificationService : INotificationService
{
    public NotificationService(ISetting setting, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _setting = setting;

        _notiRepository = unitOfWork.GetRepository<Notification>();
        _mapper = mapper;
    }

    public async Task<PagedResponse<NotificationModel>> GetNotificationByReceiverAsync(NotificationR request)
    {
        var userId = request.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var offset = request.PageSize * (request.PageNumber - 1);
        var multi = await _notiRepository.Connection.QueryMultipleAsync(GetNotificationByUserQuery,
                                                                        new
                                                                        {
                                                                            ReceiverId = userId,
                                                                            request.PageSize,
                                                                            Offet = offset
                                                                        });

        var items = await multi.ReadAsync<NotificationQueryResult>().ConfigureAwait(false);

        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);

            var resDto = _mapper.Map<List<NotificationModel>>(items);

            var response = new PagedResponse<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {
            return new PagedResponse<NotificationModel>(0);
        }
    }

    public async Task<PagedResponse<NotificationModel>> GetUnReadNotificationByReceiverAsync(NotificationR request)
    {
        var userId = request.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var offset = request.PageSize * (request.PageNumber - 1);
        var multi = await _notiRepository.Connection.QueryMultipleAsync(GetNotificationUnReadByUserQuery,
                                                                        new
                                                                        {
                                                                            ReceiverId = userId,
                                                                            request.PageSize,
                                                                            Offet = offset
                                                                        });

        var items = await multi.ReadAsync<NotificationQueryResult>().ConfigureAwait(false);
        if (items != null)
        {
            var totalItems = await multi.ReadFirstAsync<int>().ConfigureAwait(false);
            var resDto = _mapper.Map<List<NotificationModel>>(items);

            var response = new PagedResponse<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {

            return new PagedResponse<NotificationModel>(0);
        }
    }

    public async Task<bool> ReadAllNotificationAsync(Guid? userId)
    {
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var result = await _notiRepository.Connection.ExecuteAsync(UpdateNotificationStatusQuery, new
        {
            Status = NotificationStatus.Read,
            ReceiverId = userId
        });

        return result > 0;
    }

    public async Task<bool> ReadNotificationAsync(NotificationUpdateR request)
    {
        var userId = request.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var notification = await _notiRepository.GetByIdAsync(request.NotificationId);
        if (notification == null)
        {
            throw new NotFoundException(ErrorCodes.QueryEmpty, ErrorCodes.QueryEmpty);
        }

        notification.Status = NotificationStatus.Read;
        notification.ModifiedOn = DateTime.UtcNow;
        notification.ModifiedBy = userId;

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

    private readonly IRepository<Notification> _notiRepository;
    private readonly IMapper _mapper;

    #endregion
}
