using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Hubs;

using Common.Constants;
using Common.Core.Enums;
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
        await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
    }

    /// <summary>
    /// Called when a connection with the hub is terminated.
    /// </summary>
    /// <returns>A <see cref="Task"/> that represents the asynchronous disconnect.</returns>
    public override Task OnDisconnectedAsync(Exception? exception)
    {
        return base.OnDisconnectedAsync(exception);
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
    public CommentHub(IComicCommentService comicCommentService, IComicReplyService comicReplyService, IDocumentCommentService documentCommentService, IDocumentReplyService documentReplyService, ISocialCommentService socialCommentService, ISocialReplyService socialReplyService, IStoryCommentService storyCommentService, IStoryReplyService storyReplyService)
    {
        _comicCommentService = comicCommentService;
        _comicReplyService = comicReplyService;

        _documentCommentService = documentCommentService;
        _documentReplyService = documentReplyService;

        _socialCommentService = socialCommentService;
        _socialReplyService = socialReplyService;

        _storyCommentService = storyCommentService;
        _storyReplyService = storyReplyService;
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

    #endregion
}
