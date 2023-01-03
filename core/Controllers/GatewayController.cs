using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class GatewayController : ControllerBase
{
    [Route("/bot/{resource}")]
    [Authorize]
    public async Task<IActionResult> bot()
    {

        using (StreamReader stream = new StreamReader(HttpContext.Request.Body))
        {
            string body = await stream.ReadToEndAsync();
            // body = "param=somevalue&param2=someothervalue"
            Console.WriteLine(body);
        }

        return Ok(Request.GetDisplayUrl() + " | " + Request.Path);
    }

    [HttpGet]
    [Route("/bot/nebot")]
    [AllowAnonymous]
    public IActionResult authbot()
    {
        return Ok("!");
    }
}