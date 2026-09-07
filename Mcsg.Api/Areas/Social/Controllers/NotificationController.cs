using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Areas.Social.Controllers;

using Common.Core.Requests;
using Mcsg.Api.Areas.Social.Interfaces;
using Mcsg.Api.Areas.Social.Requests;

[ApiController]
[Route("api/social/[controller]")]
public class NotificationController : ControllerBase
{
    #region -- Methods --

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    [HttpGet("list")]
    [Authorize]
    public async Task<IActionResult> GetNotificationByReceiver([FromQuery] NotificationR request)
    {
        request.Analyze(HttpContext);
        var result = await _notificationService.GetNotificationByReceiverAsync(request);
        return Ok(result);
    }

    [HttpGet("unread")]
    [Authorize]
    public async Task<IActionResult> GetUnReadNotificationByReceiver([FromQuery] NotificationR request)
    {
        request.Analyze(HttpContext);
        var result = await _notificationService.GetUnReadNotificationByReceiverAsync(request);
        return Ok(result);
    }

    [HttpPut("read")]
    [Authorize]
    public async Task<IActionResult> ReadNotificationByReceiver([FromBody] NotificationUpdateR request)
    {
        request.Analyze(HttpContext);
        var result = await _notificationService.ReadNotificationAsync(request);
        return Ok(result);
    }

    [HttpPut("read-all")]
    [Authorize]
    public async Task<IActionResult> ReadAllNotificationByReceiver()
    {
        var req = new BaseR(HttpContext);
        var result = await _notificationService.ReadAllNotificationAsync(req.UserId);
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly INotificationService _notificationService;

    #endregion
}
