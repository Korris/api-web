using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Realtime.Controllers;

using Common.Core.Requests;
using Common.Models.RealTime;
using Mcsg.Api.Areas.Realtime.Interfaces;
using Mcsg.Api.Areas.Realtime.Requests;
using static Common.SeedWork.Constants.Setting;

[ApiController]
[Route("api/realtime/[controller]")]
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

    /// <summary>
    /// AddDeletion
    /// </summary>
    [HttpPost("AddDeletion"), Authorize(Policy = Policy.Admin)]
    public async Task<IActionResult> AddDeletion([FromBody] NotificationAddDeletionR request)
    {
        request.Analyze(HttpContext);

        var response = await _notificationService.AddDeletion(request);

        return Ok(response);
    }

    /// <summary>
    /// AddLock
    /// </summary>
    [HttpPost("AddLock"), Authorize(Policy = Policy.Admin)]
    public async Task<IActionResult> AddLock([FromBody] NotificationAddLockR request)
    {
        request.Analyze(HttpContext);

        var response = await _notificationService.AddLock(request);

        return Ok(response);
    }

    /// <summary>
    /// AddRejection
    /// </summary>
    [HttpPost("AddRejection"), Authorize(Policy = Policy.Admin)]
    public async Task<IActionResult> AddRejection([FromBody] NotificationAddRejectionR request)
    {
        request.Analyze(HttpContext);

        var response = await _notificationService.AddRejecton(request);

        return Ok(response);
    }

    /// <summary>
    /// RemindExpiredSubscription
    /// </summary>
    [HttpPost("RemindExpiredSubscription")]
    public async Task<IActionResult> RemindExpiredSubscription([FromBody] RemindExpiredSubscriptionR request)
    {
        request.Analyze(HttpContext);

        await _notificationService.RemindExpiredSubscriptionNotification(request);

        return Ok();
    }

    /// <summary>
    /// ExpiredSubscription
    /// </summary>
    [HttpPost("ExpiredSubscription")]
    public async Task<IActionResult> ExpiredSubscription([FromBody] ExpiredSubscriptionR request)
    {
        request.Analyze(HttpContext);

        await _notificationService.ExpiredSubscriptionNotification(request);

        return Ok();
    }

    /// <summary>
    /// AddSubPost
    /// </summary>
    [HttpPost("AddSubPost")]
    public async Task<IActionResult> AddSubPostNotification([FromBody] NotificationAddSubPostR request)
    {
        request.Analyze(HttpContext);

        await _notificationService.AddSubPostNotification(request);

        return Ok();
    }
}
