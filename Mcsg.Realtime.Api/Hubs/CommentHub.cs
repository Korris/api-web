using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

namespace Mcsg.Realtime.Api.Hubs
{
    using Common.Core.Enums;
    using Lib.Common.Constants;
    using Requests;
    using Services;

    public class CommentHub : Hub
    {
        private readonly CommentService _commentService;
        private readonly ComicCommentService _comicCommentService;
        private readonly StoryCommentService _storyCommentService;
        private readonly ReplyService _replyService;
        private readonly ComicReplyService _comicReplyService;
        private readonly StoryReplyService _storyReplyService;
        public CommentHub(
                CommentService commentService,
                ComicCommentService comicCommentService,
                StoryCommentService storyCommentService,
                ReplyService replyService,
                ComicReplyService comicReplyService,
                StoryReplyService storyReplyService)
        {
            _commentService = commentService;
            _comicCommentService = comicCommentService;
            _storyCommentService = storyCommentService;
            _storyReplyService = storyReplyService;
            _comicReplyService = comicReplyService;
            _replyService = replyService;
        }

        public override async Task OnConnectedAsync()
        {
            await Clients.All.SendAsync("onConnected", $"ClientID: {Context.ConnectionId}");
        }

        /// <summary>
        /// Send comment to post / subpost
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="commentText"></param>
        /// <param name="postId"></param>
        /// <param name="resourceHashId"></param>
        /// <param name="type">post / subpost</param>
        /// <returns></returns>
        //[Authorize]
        public async Task SendComment(PostCommentReq req)
        {
            ICommentService service = req.MicroService switch
            {
                nameof(MicroService.Comic) => _comicCommentService,
                nameof(MicroService.Story) => _storyCommentService,
                _ => _commentService
            };
            // Handle and store the new comment in database

            var resp = await service.PostComment(req);
            // Then broadcast the comment to all connected clients
            await Clients.All.SendAsync(RealTimeTopic.ReceiveComment, JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// Update comment to post / subpost
        /// </summary>
        /// <param name="id"></param>
        /// <param name="postId"></param>
        /// <param name="commentText"></param>
        /// <param name="postId"></param>
        /// <param name="resourceHashId"></param>
        /// <param name="type">post / subpost</param>
        /// <returns></returns>
        //[Authorize]
        public async Task UpdateComment(UpdateCommentReq req)
        {
            // Handle and update comment in database
            ICommentService service = req.MicroService switch
            {
                nameof(MicroService.Comic) => _comicCommentService,
                nameof(MicroService.Story) => _storyCommentService,
                _ => _commentService
            };
            var resp = await service.UpdateComment(req);

            // Then broadcast the comment to all connected clients
            await Clients.All.SendAsync(RealTimeTopic.ReceiveUpdateComment, JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// Delete comment in post / subpost
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">post / subpost</param>
        /// <returns></returns>
        //[Authorize]
        public async Task DeleteComment(DeleteCommentReq req)
        {
            ICommentService service = req.MicroService switch
            {
                nameof(MicroService.Comic) => _comicCommentService,
                nameof(MicroService.Story) => _storyCommentService,
                _ => _commentService
            };
            // Handle and update comment in database
            var resp = await _commentService.DeleteComment(req);

            // Then broadcast the comment to all connected clients
            await Clients.All.SendAsync(RealTimeTopic.ReceiveDeleteComment, JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// Reply comment to comment in post / subpost
        /// </summary>
        /// <param name="replyToCommentId"></param>
        /// <param name="replyText"></param>
        /// <param name="postId"></param>
        /// <param name="resourceHashId"></param>
        /// <param name="type">post / subpost</param>
        /// <returns></returns>
        //[Authorize]
        public async Task SendReply(ReplyCommentReq req)
        {
            IReplyService service = req.MicroService switch
            {
                nameof(MicroService.Comic) => _comicReplyService,
                nameof(MicroService.Story) => _storyReplyService,
                _ => _replyService
            };
            // Handle and store the new reply in database
            var resp = await _replyService.ReplyComment(req);

            // Then broadcast the reply to the clients of the comment
            await Clients.All.SendAsync(RealTimeTopic.ReceiveReply, JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// Update reply comment to comment in post / subpost
        /// </summary>
        /// <param name="id"></param>
        /// <param name="replyToCommentId"></param>
        /// <param name="replyText"></param>
        /// <param name="postId"></param>
        /// <param name="resourceHashId"></param>
        /// <param name="type">post / subpost</param>
        /// <returns></returns>
        //[Authorize]
        public async Task UpdateReply(UpdateReplyCommentReq req)
        {
            IReplyService service = req.MicroService switch
            {
                nameof(MicroService.Comic) => _comicReplyService,
                nameof(MicroService.Story) => _storyReplyService,
                _ => _replyService
            };
            // Handle and store the new reply in database
            var resp = await _replyService.UpdateReplyComment(req);

            // Then broadcast the reply to the clients of the comment
            await Clients.All.SendAsync(RealTimeTopic.ReceiveUpdateReply, JsonConvert.SerializeObject(resp));
        }

        /// <summary>
        /// Reply comment to comment in post / subpost
        /// </summary>
        /// <param name="replyCommentId"></param>
        /// <param name="replyToCommentId"></param>
        /// <param name="type">post / subpost</param>
        /// <returns></returns>
        //[Authorize]
        public async Task DeleteReply(DeleteReplyCommentReq req)
        {
            IReplyService service = req.MicroService switch
            {
                nameof(MicroService.Comic) => _comicReplyService,
                nameof(MicroService.Story) => _storyReplyService,
                _ => _replyService
            };
            // Handle and store the new reply in database
            var resp = await _replyService.DeleteReplyComment(req);

            // Then broadcast the reply to the clients of the comment
            await Clients.All.SendAsync(RealTimeTopic.ReceiveDeleteReply, JsonConvert.SerializeObject(resp));
        }
    }
}
