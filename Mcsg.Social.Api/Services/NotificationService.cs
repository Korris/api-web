using AutoMapper;
using Dapper;
using Grpc.Net.Client;
using Newtonsoft.Json;

namespace Mcsg.Social.Api.Services;

using Common.Core.Constants;
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
using Wallet.Api.Protos;
using static Common.SeedWork.Constants.Error;
using static Common.SeedWork.Constants.Message;

public partial class NotificationService : INotificationService
{
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

    public async Task<PagedResponse<NotificationModel>> GetNotificationByReceiverAsync(NotificationR request)
    {
        var userId = request.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var offset = request.PageSize * (request.PageNumber - 1);
        var query = GetNotificationByUserQuery;
        query = query.Replace("[UnreadCondition]", "");
        query = query.Replace("[UnreadCountCondition]", "");
        var multi = await _notiRepository.Connection.QueryMultipleAsync(query,
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
            await CheckDataReplyComment(resDto);
            await CheckDataCommentOnSubPost(resDto);
            await CheckDataFollowPost(resDto);
            await CheckDataCommentReaction(resDto);
            await CheckDataReplyCommentReaction(resDto);
            await CheckDataFollowUser(resDto);
            await CheckDataTransaction(resDto);
            var response = new PagedResponse<NotificationModel>(totalItems, request.PageNumber, request.PageSize);
            response.Items = resDto;

            return response;
        }
        else
        {
            return new PagedResponse<NotificationModel>(0);
        }
    }

    private async Task CheckDataTransaction(List<NotificationModel> resDto)
    {
        var listData = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.TransferTransaction ||
                                         p.NotificationEntityType == NotificationEntityType.DonateTransaction)
            .ToList();

        if (listData.Any())
        {
            var data = await GetTransactionFromProto(listData.Select(p => p.EntityId).Distinct().ToList());
            if (data.Any())
            {
                foreach (var item in listData)
                {
                    var transactionData = data.GetValueOrDefault(item.EntityId.ToString());
                    var amount = transactionData.Amount.ToString("N0");
                    if (item.NotificationEntityType == NotificationEntityType.DonateTransaction)
                    {
                        item.Message = string.Format(NotificationContent.DonateTransaction, item.ActorName);
                    }
                    else
                    {
                        item.Message = string.Format(NotificationContent.TransferTransaction, amount, item.ActorName);
                    }
                    item.Amount = amount;
                    item.ReferenceNumber = transactionData.ReferenceNumber;
                }

            }
        }
    }
    private async Task CheckDataReplyCommentReaction(List<NotificationModel> resDto)
    {
        var resReplyCommentReaction = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.PostCommentReplyReaction ||
                                                       p.NotificationEntityType == NotificationEntityType.SubPostCommentReplyReaction ||
                                                       p.NotificationEntityType == NotificationEntityType.ComicPostCommentReplyReaction ||
                                                       p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentReplyReaction ||
                                                       p.NotificationEntityType == NotificationEntityType.StoryPostCommentReplyReaction ||
                                                       p.NotificationEntityType == NotificationEntityType.StorySubPostCommentReplyReaction
                                                       )
                                            .ToList();

        var groupRes = resReplyCommentReaction.GroupBy(p => p.NotificationEntityType).ToList();

        foreach (var item in groupRes)
        {
            var replyCommentIds = item.Select(p => p.LocationId).ToList();
            if (replyCommentIds.Count > 0)
            {
                var tableName = item.Key switch
                {
                    NotificationEntityType.StoryPostCommentReplyReaction => $@"story.""StoryPostComments""",
                    NotificationEntityType.ComicPostCommentReplyReaction => $@"Comic.""ComicPostComments""",
                    NotificationEntityType.PostCommentReplyReaction => $@"Social.""SocialPostComments""",
                    NotificationEntityType.SubPostCommentReplyReaction => $@"Social.""SocialSubPostComments""",
                    NotificationEntityType.ComicSubPostCommentReplyReaction => $@"comic.""ComicSubPostComments""",
                    NotificationEntityType.StorySubPostCommentReplyReaction => $@"story.""StorySubPostComments""",
                    _ => ""
                };

                var query = $@"SELECT pc.""Id"" as ReplyCommentId, pc.""ParentId"" as CommentId , p.""HashId"" as LocationHashId 
                               FROM {tableName} pc
                               LEFT JOIN {tableName.Replace("Comments", "s")} p on pc.""PostId"" = p.""Id""
                               WHERE pc.""Id"" = ANY(@ids)";

                if (item.Key == NotificationEntityType.ComicSubPostCommentReplyReaction || item.Key == NotificationEntityType.StorySubPostCommentReplyReaction)
                {
                    var postTable = item.Key switch
                    {
                        NotificationEntityType.ComicSubPostCommentReplyReaction => $@"comic.""ComicPosts""",
                        _ => $@"story.""StoryPosts"""
                    };

                    query = $@"SELECT pc.""Id"" as ReplyCommentId, pc.""ParentId"" as CommentId , p.""HashId"" as LocationHashId , sp.""Order"" 
                               FROM {tableName} pc
                               LEFT JOIN {tableName.Replace("Comments", "s")} sp on pc.""PostId"" = sp.""Id""
                               LEFT JOIN {postTable} as p on p.""Id"" = sp.""PostId""
                               WHERE pc.""Id"" = ANY(@ids)";
                }

                var replyCommentData = await _notiRepository.Connection.QueryAsync<ReplyCommentReactionData>(query, new { ids = replyCommentIds });

                if (replyCommentData.Any())
                {
                    var resNeedToMap = resReplyCommentReaction.Where(p => p.NotificationEntityType == item.Key);
                    foreach (var reply in resNeedToMap)
                    {
                        var data = replyCommentData.FirstOrDefault(p => p.ReplyCommentId == reply.LocationId);
                        if (data != null)
                        {
                            reply.ReplyCommentId = data.ReplyCommentId;
                            reply.CommentId = data.CommentId;
                            reply.LocationHashId = data.LocationHashId;
                            reply.Order = data.Order;
                        }
                    }
                }
            }
        }
    }

    private async Task CheckDataCommentReaction(List<NotificationModel> resDto)
    {
        var postCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.PostCommentReaction
                                                     || p.NotificationEntityType == NotificationEntityType.PostCommentMention).Select(p => p.LocationId).ToList();
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

        var comicPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.ComicPostCommentReaction
                                                          || p.NotificationEntityType == NotificationEntityType.ComicPostCommentMention).Select(p => p.LocationId).ToList();
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

        var storyPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StoryPostCommentReaction
                                                         || p.NotificationEntityType == NotificationEntityType.StoryPostCommentMention).Select(p => p.LocationId).ToList();
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

        var comicSubPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentReaction
                                                            || p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentMention).Select(p => p.LocationId).ToList();
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
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
                    if (response != null)
                    {
                        response.LocationHashId = item.HashPostId;
                        response.CommentId = item.CommentId;
                        response.Order = item.Order;
                    }
                }
            }
        }

        var storySubPostCommentReactionIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StorySubPostCommentReaction
                                                       || p.NotificationEntityType == NotificationEntityType.StorySubPostCommentMention).Select(p => p.LocationId).ToList();
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
                    var response = resDto.FirstOrDefault(p => p.LocationId == item.CommentId);
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
        var resDtoFollowComic = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.FollowComicPost).ToList();
        var followComicPostIds = resDtoFollowComic.Select(p => p.LocationId).ToList();
        if (followComicPostIds.Count > 0)
        {
            var comics = await _notiRepository.Connection.QueryAsync<PostData>($@"
                                        SELECT cp.""Title"",cp.""HashId""  from comic.""ComicPosts"" cp
                                        WHERE cp.""Id"" = ANY(@ids)", new { ids = followComicPostIds });
            if (comics.Count() > 0)
            {
                foreach (var item in resDtoFollowComic)
                {
                    var comic = comics.FirstOrDefault(p => p.HashId == item.LocationHashId);
                    if (comic != null)
                    {
                        item.Message = string.Format(NotificationContent.FollowPost, item.ActorName, comic.Title);
                    }
                }
            }
        }
        var resDtoFollowStory = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.FollowStoryPost).ToList();
        var followStoryPostIds = resDtoFollowStory.Select(p => p.LocationId).ToList();
        if (followStoryPostIds.Count > 0)
        {
            var stories = await _notiRepository.Connection.QueryAsync<PostData>($@"
                                        SELECT sp.""Title"",sp.""HashId""  from story.""StoryPosts"" sp
                                        WHERE sp.""Id"" = ANY(@ids)", new { ids = followStoryPostIds });
            if (stories.Count() > 0)
            {
                foreach (var item in resDtoFollowStory)
                {
                    var story = stories.FirstOrDefault(p => p.HashId == item.LocationHashId);
                    if (story != null)
                    {
                        item.Message = string.Format(NotificationContent.FollowPost, item.ActorName, story.Title);
                    }
                }
            }
        }
    }

    private async Task CheckDataReplyComment(List<NotificationModel> resDto)
    {
        var res = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StoryPostCommentReply ||
                                    p.NotificationEntityType == NotificationEntityType.StorySubPostCommentReply ||
                                    p.NotificationEntityType == NotificationEntityType.ComicPostCommentReply ||
                                    p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentReply ||
                                    p.NotificationEntityType == NotificationEntityType.PostCommentReply ||
                                    p.NotificationEntityType == NotificationEntityType.SubPostCommentReply
        ).ToList();

        var groupRes = res.GroupBy(p => p.NotificationEntityType).ToList();

        foreach (var item in groupRes)
        {
            var replyCommentIds = item.Select(p => p.EntityId).ToList();
            if (replyCommentIds.Count > 0)
            {
                var tableName = item.Key switch
                {
                    NotificationEntityType.StoryPostCommentReply => $@"story.""StoryPostComments""",
                    NotificationEntityType.StorySubPostCommentReply => $@"story.""StorySubPostComments""",
                    NotificationEntityType.ComicPostCommentReply => $@"Comic.""ComicPostComments""",
                    NotificationEntityType.ComicSubPostCommentReply => $@"Comic.""ComicSubPostComments""",
                    NotificationEntityType.PostCommentReply => $@"Social.""SocialPostComments""",
                    NotificationEntityType.SubPostCommentReply => $@"Social.""SocialSubPostComments""",
                    _ => ""
                };
                var replyCommentData = await _notiRepository.Connection.QueryAsync<ReplyCommentData>($@"
                                        SELECT ""Id"" as ReplyCommentId, ""ParentId"" as CommentId
                                        FROM {tableName} 
                                        WHERE ""Id"" = ANY(@ids)", new { ids = replyCommentIds });

                if (replyCommentData.Any())
                {
                    var resNeedToMap = res.Where(p => p.NotificationEntityType == item.Key);
                    foreach (var reply in replyCommentData)
                    {
                        var data = resNeedToMap.FirstOrDefault(p => p.EntityId == reply.ReplyCommentId);
                        if (data != null)
                        {
                            data.ReplyCommentId = reply.ReplyCommentId;
                            data.CommentId = reply.CommentId;
                        }
                    }
                }
            }
        }
    }

    private async Task CheckDataCommentOnSubPost(List<NotificationModel> resDto)
    {
        var subComicIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.ComicSubPostComment || p.NotificationEntityType == NotificationEntityType.ComicSubPostCommentReply).Select(p => p.LocationId).ToList();
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

        var subStoryIds = resDto.Where(p => p.NotificationEntityType == NotificationEntityType.StorySubPostComment || p.NotificationEntityType == NotificationEntityType.StorySubPostCommentReply).Select(p => p.LocationId).ToList();
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
        var userId = request.UserId;
        if (userId == null)
        {
            throw new NotFoundException(E303, M303);
        }

        var offset = request.PageSize * (request.PageNumber - 1);
        var query = GetNotificationByUserQuery;
        query = query.Replace("[UnreadCondition]", $@"AND noti.""Status"" = 0");
        query = query.Replace("[UnreadCountCondition]", $@"AND noti.""Status"" = 0");
        var multi = await _notiRepository.Connection.QueryMultipleAsync(query,
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

    public async Task<bool> AddMentionNotificationAsync(MentionPostNotificationReq req)
    {
        var baseUrl = _setting.Api.Web.Realtime;
        var urlBuilder = new System.Text.StringBuilder();
        urlBuilder.Append(baseUrl != null ? baseUrl.TrimEnd('/') : "").Append("/notification/post-mention");

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

    private async Task CheckDataFollowUser(List<NotificationModel> resDto)
    {
        var userFollowIds = resDto
            .Where(p => p.NotificationEntityType == NotificationEntityType.FollowUser)
            .Select(p => p.EntityId)
            .ToList();

        if (userFollowIds.Count > 0)
        {
            var userData = await _notiRepository.Connection.QueryAsync<UserFollowedResponse>($@"
                SELECT u.""Id"" as UserId,  
                       u.""ProfileName"", 
                       u.""UserName"", 
                       u.""Avatar""
                FROM identity.""Users"" u
                WHERE u.""Id"" = ANY(@ids)", new { ids = userFollowIds });

            if (userData.Count() > 0)
            {
                foreach (var item in userData)
                {
                    var response = resDto.FirstOrDefault(p => p.EntityId == item.UserId);
                    if (response != null)
                    {
                        response.LocationHashId = item.UserName + "";
                    }
                }
            }
        }
    }

    private async Task<Dictionary<string, TransactionProtoDto>> GetTransactionFromProto(List<Guid?> transactionIds)
    {
        var res = new Dictionary<string, TransactionProtoDto>();

        try
        {
            using var channel = GrpcChannel.ForAddress(_setting.Rpc.Web.Wallet!);

            var client = new UserWalletProto.UserWalletProtoClient(channel);
            var request = new TransactionGetReq
            {
                TransactionId = string.Join(';', transactionIds.Where(id => id != null).Select(id => id.ToString()))
            };
            var rsp = await client.GetTransactionInfoAsync(request);
            return rsp.Transactions.ToDictionary(p => p.TransactionId, p => p);
        }
        catch (Exception ex)
        {
            ex.Message.LogError();
        }

        return res;
    }

    #region -- Fields --

    /// <summary>
    /// Setting
    /// </summary>
    private readonly ISetting _setting;

    #endregion
}
