using Mcsg.Api.DTOs;
using Mcsg.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet("list")]
        [Authorize]
        public async Task<IActionResult> GetNotificationByReceiver([FromQuery] NotificationReq request)
        {
            var result = await _notificationService.GetNotificationByReceiverAsync(request);
            return Ok(result);
        }

        [HttpGet("unread")]
        [Authorize]
        public async Task<IActionResult> GetUnReadNotificationByReceiver([FromQuery] NotificationReq request)
        {
            var result = await _notificationService.GetUnReadNotificationByReceiverAsync(request);
            return Ok(result);
        }

        [HttpPut("read")]
        [Authorize]
        public async Task<IActionResult> ReadNotificationByReceiver([FromBody] UpdateNotificationReq request)
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
    }
}
