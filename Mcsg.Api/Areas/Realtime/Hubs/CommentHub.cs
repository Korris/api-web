using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Api.Areas.Realtime.Hubs;

using Common.Constants;
using Common.Core.Enums;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;

/// <summary>
/// Comment hub
/// </summary>
public class CommentHub : Hub
{
    #region -- Overrides --

    /// <summary>
    /// Called when a new connection is established with the hub.
    /// </summary>
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    #endregion

    #region -- Methods --

    /// <summary>
    /// Initialize
    /// </summary>
    public CommentHub(
        IComicCommentService comicCommentService,
        IComicReplyService comicReplyService,
        IDocumentCommentService documentCommentService,
        IDocumentReplyService documentReplyService,
        ISocialCommentService socialCommentService,
        ISocialReplyService socialReplyService,
        IStoryCommentService storyCommentService,
        IStoryReplyService storyReplyService,
        ITapShowCommentService tapShowCommentService,
        ITapShowReplyService tapShowReplyService)
    {
        _comicCommentService = comicCommentService;
        _comicReplyService = comicReplyService;
        _documentCommentService = documentCommentService;
        _documentReplyService = documentReplyService;
        _socialCommentService = socialCommentService;
        _socialReplyService = socialReplyService;
        _storyCommentService = storyCommentService;
        _storyReplyService = storyReplyService;
        _tapShowCommentService = tapShowCommentService;
        _tapShowReplyService = tapShowReplyService;
    }

    /// <summary>
    /// Send comment to post/subpost
    /// </summary>
    [Authorize]
    public async Task SendComment(PostCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetCommentService(req.MicroService);
        var resp = await service.PostComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, false);
        await Clients.All.SendAsync(RealTimeTopic.ReceiveComment, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Update comment to post/subpost
    /// </summary>
    [Authorize]
    public async Task UpdateComment(UpdateCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetCommentService(req.MicroService);
        var resp = await service.UpdateComment(req);

        await Clients.All.SendAsync(RealTimeTopic.ReceiveUpdateComment, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Delete comment in post/subpost
    /// </summary>
    [Authorize]
    public async Task DeleteComment(DeleteCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetCommentService(req.MicroService);
        var resp = await service.DeleteComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, true);
        await Clients.All.SendAsync(RealTimeTopic.ReceiveDeleteComment, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Reply comment to comment in post/subpost
    /// </summary>
    [Authorize]
    public async Task SendReply(ReplyCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetReplyService(req.MicroService);
        var resp = await service.ReplyComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, false);
        await Clients.All.SendAsync(RealTimeTopic.ReceiveReply, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Update reply comment
    /// </summary>
    [Authorize]
    public async Task UpdateReply(UpdateReplyCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetReplyService(req.MicroService);
        var resp = await service.UpdateReplyComment(req);

        await Clients.All.SendAsync(RealTimeTopic.ReceiveUpdateReply, JsonConvert.SerializeObject(resp));
    }

    /// <summary>
    /// Delete reply comment
    /// </summary>
    [Authorize]
    public async Task DeleteReply(DeleteReplyCommentReq req)
    {
        req.Analyze(Context.GetHttpContext());
        var service = GetReplyService(req.MicroService);
        var resp = await service.DeleteReplyComment(req);

        await SendToAuthor(req.MicroService, req.Type, resp.Id, resp.PostId, resp.PostIdOfPost, true);
        await Clients.All.SendAsync(RealTimeTopic.ReceiveDeleteReply, JsonConvert.SerializeObject(resp));
    }

    private ISocialCommentService GetCommentService(string microService)
    {
        return microService switch
        {
            nameof(MicroService.Comic) => _comicCommentService,
            nameof(MicroService.Document) => _documentCommentService,
            nameof(MicroService.Story) => _storyCommentService,
            nameof(MicroService.TapShow) => _tapShowCommentService,
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
            nameof(MicroService.TapShow) => _tapShowReplyService,
            _ => _socialReplyService
        };
    }

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
    private readonly ITapShowCommentService _tapShowCommentService;
    private readonly ITapShowReplyService _tapShowReplyService;

    #endregion
}
