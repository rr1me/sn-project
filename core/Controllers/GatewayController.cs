using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class GatewayController : ControllerBase
{
    [Route("/bot/{resource}")]
    [Authorize]
    public IActionResult bot()
    {

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