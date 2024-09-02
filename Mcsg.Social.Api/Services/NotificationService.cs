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
    private readonly IRepository<User> _userRepository;
    private readonly IRepository<SocialPost> _postRepository;

    private readonly IRepository<SocialPostReaction> _postReactionRepository;
    private readonly IRepository<ComicPostReaction> _comicPostReactionRepository;
    private readonly IRepository<StoryPostReaction> _storyPostReactionRepository;

    private readonly IRepository<SocialSubPostReaction> _subPostReactionRepository;

    private readonly IRepository<SocialPostCommentReaction> _postCommentReactionRepository;
    private readonly IRepository<ComicPostCommentReaction> _comicPostCommentReactionRepository;
    private readonly IRepository<StoryPostCommentReaction> _storyPostCommentReactionRepository;

    private readonly IRepository<SocialSubPostCommentReaction> _subPostCommentReactionRepository;
    private readonly IRepository<ComicSubPostCommentReaction> _comicSubPostCommentReactionRepository;
    private readonly IRepository<StorySubPostCommentReaction> _storySubPostCommentReactionRepository;


    private readonly IMapper _mapper;
    private IConfiguration _configuration;

    public NotificationService(ICurrentUserService currentUserService
        , IUnitOfWork unitOfWork
        , IMapper mapper
        , IConfiguration configuration
        , ISetting setting
        , IRepository<User> userRepository)
    {
        _currentUserService = currentUserService;
        _notiRepository = unitOfWork.GetRepository<Notification>();
        _notiObjRepository = unitOfWork.GetRepository<NotificationObject>();
        _userRepository = unitOfWork.GetRepository<User>();
        _postRepository = unitOfWork.GetRepository<SocialPost>();
        _mapper = mapper;
        _setting = setting;
        _configuration = configuration;

        _postReactionRepository = unitOfWork.GetRepository<SocialPostReaction>();
        _comicPostReactionRepository = unitOfWork.GetRepository<ComicPostReaction>();
        _storyPostReactionRepository = unitOfWork.GetRepository<StoryPostReaction>();

        _subPostReactionRepository = unitOfWork.GetRepository<SocialSubPostReaction>();

        _postCommentReactionRepository = unitOfWork.GetRepository<SocialPostCommentReaction>();
        _comicPostCommentReactionRepository = unitOfWork.GetRepository<ComicPostCommentReaction>();
        _storyPostCommentReactionRepository = unitOfWork.GetRepository<StoryPostCommentReaction>();
        _subPostCommentReactionRepository = unitOfWork.GetRepository<SocialSubPostCommentReaction>();
        _comicSubPostCommentReactionRepository = unitOfWork.GetRepository<ComicSubPostCommentReaction>();
        _storySubPostCommentReactionRepository = unitOfWork.GetRepository<StorySubPostCommentReaction>();
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
        var query = GetNotificationByUserQuery;
        query = query.Replace("[UnreadCondition]", "");
        query = query.Replace("[UnreadCountCondition]", "");
        var multi = await _notiRepository.Connection.QueryMultipleAsync(query,
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
            await CheckDataCommentOnSubPost(resDto);
            await CheckDataFollowPost(resDto);
            await CheckDataCommentReaction(resDto);
            var response = new PagedResponse<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {
            return new PagedResponse<NotificationModel>(0);
        }
    }

    private async Task CheckDataCommentReaction(List<NotificationModel> resDto)
    {
        var postCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.PostCommentReaction).Select(p => p.LocationId).ToList();
        if (postCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                        SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from social.""SocialPostComments"" pc
                                        LEFT JOIN social.""SocialPosts"" sp on pc.""PostId"" = sp.""Id""
                                        WHERE pc.""Id"" = ANY(@ids)", new { ids = postCommentReactionIds });
            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                    }
                }
            }
        }

        var comicPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.ComicPostCommentReaction).Select(p => p.LocationId).ToList();
        if (comicPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                        SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from comic.""ComicPostComments"" pc
                                        LEFT JOIN comic.""ComicPosts"" sp on pc.""PostId"" = sp.""Id""
                                        WHERE pc.""Id"" = ANY(@ids)", new { ids = comicPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                    }
                }
            }
        }

        var storyPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StoryPostCommentReaction).Select(p => p.LocationId).ToList();
        if (storyPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                  SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from story.""StoryPostComments"" pc
                                  LEFT JOIN story.""StoryPosts"" sp on pc.""PostId"" = sp.""Id""
                                  WHERE pc.""Id"" = ANY(@ids)", new { ids = storyPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                    }
                }
            }
        }

        var comicSubPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentReaction).Select(p => p.LocationId).ToList();
        if (comicSubPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                        SELECT pc.""Id"" as CommentId,p.""HashId"" as HashPostId, sp.""Order"" from comic.""ComicSubPostComments"" pc
                                        LEFT JOIN comic.""ComicSubPosts"" sp on pc.""PostId"" = sp.""Id""                                        
                                        LEFT JOIN comic.""ComicPosts"" p on sp.""PostId"" = p.""Id""
                                        WHERE pc.""Id"" = ANY(@ids)", new { ids = comicSubPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId &&
                                                         p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentReaction);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                        response.Order = item.Order;
                    }
                }
            }
        }

        var storySubPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StorySubPostCommentReaction).Select(p => p.LocationId).ToList();
        if (storySubPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                 SELECT pc.""Id"" as CommentId,p.""HashId"" as HashPostId, sp.""Order"" from story.""StorySubPostComments"" pc
                                 LEFT JOIN story.""StorySubPosts"" sp on pc.""PostId"" = sp.""Id""                                 
                                 LEFT JOIN story.""StoryPosts"" p on sp.""PostId"" = p.""Id""
                                 WHERE pc.""Id"" = ANY(@ids)", new { ids = storySubPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId &&
                                                         p.NotificationEntityType == NotificationEntityType.StorySubPostCommentReaction);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                        response.Order = item.Order;
                    }
                }
            }
        }

        var subPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.SubPostCommentReaction).Select(p => p.LocationId).ToList();
        if (subPostCommentReactionIds.Count > 0)
        {
            var postDataByPostComment = await _notiRepository.Connection.QueryAsync<PostDataByPostComment>($@"
                                 SELECT pc.""Id"" as CommentId,sp.""HashId"" as HashPostId from social.""SocialSubPostComments"" pc
                                 LEFT JOIN social.""SocialSubPosts"" sp on pc.""PostId"" = sp.""Id""                                 
                                 LEFT JOIN social.""SocialPosts"" p on sp.""PostId"" = p.""Id""
                                 WHERE pc.""Id"" = ANY(@ids)", new { ids = subPostCommentReactionIds });

            if (postDataByPostComment.Count() > 0)
            {
                foreach (var item in postDataByPostComment)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId &&
                                                         p.NotificationEntityType == NotificationEntityType.SubPostCommentReaction);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                    }
                }
            }
        }
    }

    private async Task CheckDataFollowPost(List<NotificationModel> resDto)
    {
        var followComicPostIds = resDto.Where(p => p.TargetType == NotificationTargetType.FollowComicPost).Select(p => p.LocationId).ToList();
        if (followComicPostIds.Count > 0)
        {
            var comics = await _notiRepository.Connection.QueryAsync<PostData>($@"
                                        SELECT cp.""Title"",cp.""HashId""  from comic.""ComicPosts"" cp
                                        WHERE cp.""Id"" = ANY(@ids)", new { ids = followComicPostIds });
            if (comics.Count() > 0)
            {
                foreach (var item in comics)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationHashId == item.HashId);
                    if (response != null)
                    {
                        response.Message = string.Format(response.Message, response.ActorName, item.Title);
                    }
                }
            }
        }
        var followStoryPostIds = resDto.Where(p => p.TargetType == NotificationTargetType.FollowStoryPost).Select(p => p.LocationId).ToList();
        if (followStoryPostIds.Count > 0)
        {
            var stories = await _notiRepository.Connection.QueryAsync<PostData>($@"
                                        SELECT sp.""Title"",sp.""HashId""  from story.""StoryPosts"" sp
                                        WHERE sp.""Id"" = ANY(@ids)", new { ids = followStoryPostIds });
            if (stories.Count() > 0)
            {
                foreach (var item in stories)
                {
                    var response = resDto.FirstOrDefault(p => p.LocationHashId == item.HashId);
                    if (response != null)
                    {
                        response.Message = string.Format(response.Message, response.ActorName, item.Title);
                    }
                }
            }
        }
    }

    private async Task CheckDataCommentOnSubPost(List<NotificationModel> resDto)
    {
        var subComicIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.ComicSubPostComment).Select(p => p.LocationId).ToList();
        if (subComicIds.Count > 0)
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

        var subStoryIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StorySubPostComment).Select(p => p.LocationId).ToList();
        if (subStoryIds.Count > 0)
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
        var query = GetNotificationByUserQuery;
        query = query.Replace("[UnreadCondition]", $@"AND noti.""Status"" = 0");
        query = query.Replace("[UnreadCountCondition]", $@"AND ""Status"" = 0");
        var multi = await _notiRepository.Connection.QueryMultipleAsync(query,
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
            await CheckDataCommentOnSubPost(resDto);
            await CheckDataFollowPost(resDto);
            await CheckDataCommentReaction(resDto);
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
