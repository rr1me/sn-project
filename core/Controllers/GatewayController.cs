using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace core.Controllers;

[ApiController]
public class GatewayController : ControllerBase
{

    private readonly IHttpClientFactory _clientFactory;

    public GatewayController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    [Route("/bot/{resource}")]
    [Authorize]
    public async Task<IActionResult> bot(string resource)
    {

        // HttpContext.Request.EnableBuffering();
        //
        var stream = new StreamReader(HttpContext.Request.Body);
        var body = await stream.ReadToEndAsync();
        Console.WriteLine(resource);
        //
        var request = new HttpRequestMessage(HttpMethod.Post, "http://localhost:5000/"+resource);
        request.Content = new StringContent(body, Encoding.UTF8, "application/json");
        
        var httpClient = _clientFactory.CreateClient();
        
        var r = await httpClient.SendAsync(request);
        
        
        // HttpContext.Response.Redirect("http://localhost:5000/embed");
        
        
        
        // Console.WriteLine(r);
        return Ok("q");
        

        // return Ok(Request.GetDisplayUrl() + " | " + Request.Path);
    }

    [HttpGet]
    [Route("/bot/nebot")]
    [AllowAnonymous]
    public IActionResult authbot()
    {
        return Ok("!");
    }
}