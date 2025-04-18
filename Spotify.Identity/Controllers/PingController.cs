using Microsoft.AspNetCore.Mvc;

namespace Spotify.Identity.Controllers;

[ApiController]
[Route("api/ping")]
public class PingController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Pong!");
}