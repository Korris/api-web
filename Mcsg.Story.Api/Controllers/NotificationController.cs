using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Story.Api.Controllers;

using Interfaces;
using Requests;

[ApiController]
[Route("[controller]")]
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
        var result = await _notificationService.GetNotificationByReceiverAsync(request);
        return Ok(result);
    }

    [HttpGet("unread")]
    [Authorize]
    public async Task<IActionResult> GetUnReadNotificationByReceiver([FromQuery] NotificationR request)
    {
        var result = await _notificationService.GetUnReadNotificationByReceiverAsync(request);
        return Ok(result);
    }

    [HttpPut("read")]
    [Authorize]
    public async Task<IActionResult> ReadNotificationByReceiver([FromBody] NotificationUpdateR request)
    {
        var result = await _notificationService.ReadNotificationAsync(request.NotificationId);
        return Ok(result);
    }

    [HttpPut("read-all")]
    [Authorize]
    public async Task<IActionResult> ReadAllNotificationByReceiver()
    {
        var result = await _notificationService.ReadAllNotificationAsync();
        return Ok(result);
    }

    #endregion

    #region -- Fields --

    private readonly INotificationService _notificationService;

    #endregion
}
