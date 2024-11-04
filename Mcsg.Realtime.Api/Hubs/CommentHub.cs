using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Hubs;

using Common.Constants;
using Common.Core.Enums;
using Common.Core.Interfaces;
using Common.Core.Requests;
using Interfaces;
using Requests;

/// <summary>
/// Comment hub
/// </summary>
public class CommentHub : Hub
{
    #region -- Overrides --

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous connect.</returns>
    public override async Task OnConnectedAsync()
    {
        var hc = Context.GetHttpContext();
        var req = new BaseR(hc);
        var userId = req.UserId;
        var deviceType = hc?.Request.Query["deviceType"];

        await SaveConnectionDataAsync(userId != null ? userId.Value.ToString() : "", Context.ConnectionId, deviceType + "");

        await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous disconnect.</returns>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var req = new BaseR(Context.GetHttpContext());
        var userId = req.UserId;

        var key = userId != null
            ? $"user:{userId}:connection:{Context.ConnectionId}"
            : $"anon:connection:{Context.ConnectionId}";

        await _rs.RedisCache.KeyDeleteAsync(key);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    /// <param name="comicCommentService"></param>
    /// <param name="comicReplyService"></param>
    /// <param name="documentCommentService"></param>
    /// <param name="documentReplyService"></param>
    /// <param name="socialCommentService"></param>
    /// <param name="socialReplyService"></param>
    /// <param name="storyCommentService"></param>
    /// <param name="storyReplyService"></param>
    /// <param name="rs"></param>
    public CommentHub(IComicCommentService comicCommentService, IComicReplyService comicReplyService, IDocumentCommentService documentCommentService, IDocumentReplyService documentReplyService, ISocialCommentService socialCommentService, ISocialReplyService socialReplyService, IStoryCommentService storyCommentService, IStoryReplyService storyReplyService, IRedisStore rs)
    {
        _comicCommentService = comicCommentService;
        _comicReplyService = comicReplyService;

        _documentCommentService = documentCommentService;
        _documentReplyService = documentReplyService;

        _socialCommentService = socialCommentService;
        _socialReplyService = socialReplyService;

        _storyCommentService = storyCommentService;
        _storyReplyService = storyReplyService;

        _rs = rs;
    }

    /// <summary>
    /// Send comment to post/subpost
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    public async Task SendComment(PostCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetCommentService(req.MicroService);
        var resp = await service.PostComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, false);

        // Then broadcast the comment to all connected clients
        await Clients.All.SendAsync(RealTimeTopic.ReceiveComment, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Update comment to post/subpost
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    public async Task UpdateComment(UpdateCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetCommentService(req.MicroService);
        var resp = await service.UpdateComment(req);

        // Then broadcast the comment to all connected clients
        await Clients.All.SendAsync(RealTimeTopic.ReceiveUpdateComment, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Delete comment in post/subpost
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    public async Task DeleteComment(DeleteCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetCommentService(req.MicroService);
        var resp = await service.DeleteComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, true);

        // Then broadcast the comment to all connected clients
        await Clients.All.SendAsync(RealTimeTopic.ReceiveDeleteComment, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Reply comment to comment in post/subpost
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    public async Task SendReply(ReplyCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetReplyService(req.MicroService);
        var resp = await service.ReplyComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, false);

        // Then broadcast the reply to the clients of the comment
        await Clients.All.SendAsync(RealTimeTopic.ReceiveReply, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Update reply comment to comment in post/subpost
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    public async Task UpdateReply(UpdateReplyCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetReplyService(req.MicroService);
        var resp = await service.UpdateReplyComment(req);

        // Then broadcast the reply to the clients of the comment
        await Clients.All.SendAsync(RealTimeTopic.ReceiveUpdateReply, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Delete reply comment to comment in post/subpost
    /// </summary>
    /// <param name="req"></param>
    /// <returns></returns>
    [Authorize]
    public async Task DeleteReply(DeleteReplyCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetReplyService(req.MicroService);
        var resp = await service.DeleteReplyComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, true);

        // Then broadcast the reply to the clients of the comment
        await Clients.All.SendAsync(RealTimeTopic.ReceiveDeleteReply, JsonConvert.SerializeObject(resp));
    }

    private ISocialCommentService GetCommentService(string microService)
    {
        return microService switch
        {
            nameof(MicroService.Comic) => _comicCommentService,
            nameof(MicroService.Document) => _documentCommentService,
            nameof(MicroService.Story) => _storyCommentService,
            _ => _socialCommentService
        };
    }

    private ISocialReplyService GetReplyService(string microService)
    {
        return microService switch
        {
            nameof(MicroService.Comic) => _comicReplyService,
            nameof(MicroService.Document) => _documentReplyService,
            nameof(MicroService.Story) => _storyReplyService,
            _ => _socialReplyService
        };
    }

    private async Task SaveConnectionDataAsync(string userId, string contextId, string deviceType)
    {
        // Use context ID as the primary key if user ID is null
        var key = !string.IsNullOrEmpty(userId) ? $"user:{userId}:connection:{contextId}" : $"anon:connection:{contextId}";

        var data = new
        {
            ContextId = contextId,
            DeviceType = deviceType,
            UserId = userId,
            CreatedOn = DateTime.UtcNow,
        };

        await _rs.RedisCache.StringSetAsync(key, JsonConvert.SerializeObject(data));
    }

    /// <summary>
    /// Send to author to call API tracking
    /// </summary>
    /// <param name="microService"></param>
    /// <param name="type"></param>
    /// <param name="commentId"></param>
    /// <param name="postId"></param>
    /// <param name="postIdOfPost"></param>
    /// <param name="isDeleted"></param>
    /// <returns></returns>
    private async Task SendToAuthor(string microService, string type, Guid commentId, Guid postId, Guid postIdOfPost, bool isDeleted)
    {
        var isPost = type == "post";
        var response = new NotificationSuccessResponse
        {
            CommentId = commentId,
            MicroService = microService,
            PostId = isPost ? postId : postIdOfPost,
            SubPostId = isPost ? null : postId,
            IsDeleted = isDeleted
        };

        var data = JsonConvert.SerializeObject(response);
        await Clients.Client(Context.ConnectionId).SendAsync(RealTimeTopic.ReceiveSendSuccessComment, data);
    }

    #endregion

    #region -- Fields --

    private readonly IComicCommentService _comicCommentService;
    private readonly IComicReplyService _comicReplyService;

    private readonly IDocumentCommentService _documentCommentService;
    private readonly IDocumentReplyService _documentReplyService;

    private readonly ISocialCommentService _socialCommentService;
    private readonly ISocialReplyService _socialReplyService;

    private readonly IStoryCommentService _storyCommentService;
    private readonly IStoryReplyService _storyReplyService;

    private readonly IRedisStore _rs;

    #endregion
}
