using MediatR;
using Microsoft.AspNetCore.Mvc;
using Spotify.Identity.Mediatr.Commands;

namespace Spotify.Identity.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AuthController(ISender sender,ILogger<AuthController> logger) : ControllerBase
{
    [HttpPost("[action]")]
    public async Task<ActionResult> Register(RegisterCommand registerCommand)
    {
        logger.LogInformation($"Registering user {registerCommand.UserName}");
        await sender.Send(registerCommand);
        
        return Ok();
    }

    [HttpPost("[action]")]
    public async Task<ActionResult> Login(LoginCommand loginCommand)
    {
        var token=await sender.Send(loginCommand);
        
        return Ok(token);
    }

    [HttpPatch("[action]")]
    public async Task<ActionResult> UpdatePassword(UpdatePasswordCommand updatePasswordCommand)
    {
        await sender.Send(updatePasswordCommand);
        
        return Ok();
    }
  
}