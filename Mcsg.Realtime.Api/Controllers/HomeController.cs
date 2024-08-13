using Microsoft.AspNetCore.Mvc;

namespace Mcsg.Realtime.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HomeController : ControllerBase
{
    [HttpGet]
    public IActionResult Index()
    {
        return Ok("I am realtime service");
    }
}
