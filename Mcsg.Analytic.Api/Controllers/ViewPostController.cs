using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Analytic.Api.Controllers
{
    using Models;

    [ApiController]
    [Route("[controller]")]
    public class ViewPostController : ControllerBase
    {
        public ViewPostController() { }
        [HttpPost]
        public async Task<IActionResult> ViewPost([FromQuery] UserViewPostReq request)
        {
            return Ok();
        }
    }
}
