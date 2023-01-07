using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
public class GatewayController : ControllerBase
{
    private readonly IHttpClientFactory _clientFactory;

    public GatewayController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [Route("/bot/{*resource}")]
    public async Task<IActionResult> BotGateway(string resource) => await RoutingHandler("http://localhost:5000/" + resource);
    
    [Route("/non-auth/{*q}")]   
    [AllowAnonymous]
    public IActionResult TestGateway(string q)
    {
        Console.WriteLine(q);
        return Ok("!");
    }

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
}