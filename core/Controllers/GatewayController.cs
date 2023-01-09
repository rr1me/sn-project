using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class GatewayController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;
    
    private readonly IWebHostEnvironment _env;

    public GatewayController(IHttpClientFactory clientFactory, IWebHostEnvironment env)
    {
        _clientFactory = clientFactory;
        _env = env;
    }

    [Route("/bot/{*resource}")]
    public async Task<IActionResult> BotGateway(string resource) => await RoutingHandler("http://localhost:5000/" + resource);

    private async  Task<IActionResult> RoutingHandler(string url)
    {
        var stream = new StreamReader(HttpContext.Request.Body);
        var body = await stream.ReadToEndAsync();

        var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        
        var httpClient = _clientFactory.CreateClient();
        
        var r = await httpClient.SendAsync(request);
        return StatusCode((int)r.StatusCode, r.ReasonPhrase);
    }

    [HttpGet("/launcher")]
    [AllowAnonymous]
    public IActionResult GetLauncher()
    {
        var path = Path.Combine(_env.ContentRootPath, "Static/RadOsLauncher.exe");
        return File(System.IO.File.OpenRead(path), "application/octet-stream", Path.GetFileName(path));
    }
}