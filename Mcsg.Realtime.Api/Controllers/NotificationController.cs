using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Realtime.Api.Controllers;

using Common.Core.Requests;
using Common.Models.RealTime;
using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;
    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpPost("video")]
    public async Task<IActionResult> AddVideoNotification([FromBody] VideoNotificationR request)
    {
        var result = await _notificationService.AddVideoNotification(request);
        return Ok(result);
    }

    [HttpPost("reaction")]
    public async Task<IActionResult> AddReactionNotification([FromBody] ReactionNotificationReq request)
    {
        var result = await _notificationService.AddReactionNotification(request);
        return Ok(result);
    }

    [HttpPost("transaction-update")]
    public async Task<IActionResult> AddTransactionUpdateNotification([FromBody] RealTimeTransactionUpdateReq request)
    {
        await _notificationService.AddTransactionUpdate(request);
        return Ok();
    }
    [HttpPost("common-notification")]
    public async Task<IActionResult> AddCommonNotification([FromBody] CommonNotificationReq request)
    {
        await _notificationService.AddCommonNotification(request);
        return Ok();
    }

    [HttpPost("mention")]
    public async Task<IActionResult> AddMentionNotification([FromBody] CommonNotificationReq request)
    {
        await _notificationService.AddCommonNotification(request);
        return Ok();
    }

    [HttpPost("post-mention")]
    public async Task<IActionResult> AddPostMentionNotification([FromBody] MentionPostNotificationReq request)
    {
        await _notificationService.AddPostMentionNotification(request);
        return Ok();
    }

    [HttpPost("transaction")]
    public async Task<IActionResult> AddTransactionNotification([FromBody] TransactionNotificationReq request)
    {
        await _notificationService.AddTransactionNotification(request);
        return Ok();
    }
}
