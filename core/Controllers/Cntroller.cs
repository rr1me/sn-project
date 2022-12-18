using IdentityServer4;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class Cntroller : ControllerBase
{
    [HttpGet("q")]
    [Authorize(IdentityServerConstants.LocalApi.PolicyName)]
    public IActionResult Index()
    {
        return Ok("ok?");
    }
}