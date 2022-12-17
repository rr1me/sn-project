using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace discordBot.Controllers;

[ApiController]
public class BotController : ControllerBase
{
    [Authorize]
    [HttpGet("/")]
    public IActionResult Index()
    {
        Console.WriteLine("блять");
        return Ok("IO");
    }
}