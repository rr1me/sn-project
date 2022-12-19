using core;
using core.Authentication;
using core.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<GatewayAuthenticationHandler>();

builder.Services.AddDbContext<DatabaseContext>(x =>
{
    var serverVersion = ServerVersion.AutoDetect("server=localhost;user=root;password=root;database=sntestdb");
    x
        .UseMySql("server=localhost;user=root;password=root;database=sntestdb", new MySqlServerVersion(new Version(8, 0, 31)))
        .LogTo(Console.WriteLine, LogLevel.Information);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "GatewayAuthScheme";
}).AddScheme<GatewayAuthScheme, AuthenticationMiddleware>("GatewayAuthScheme", null);
builder.Services.AddAuthorization(x =>
    x.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddControllers();

builder.Services.AddHttpClient();

var app = builder.Build();

// app.UseHttpsRedirection();

app.UseRouting();

app.MapControllers();

app.Use(async (context, next) =>
{
    var t = true;
    var param = context.Request.Query["param"];
    if (param == "redirect")
    {
        var client = new HttpClient();
        var resp = await client.GetAsync("https://localhost:7260/");
        await context.Response.WriteAsync(new StreamReader(resp.Content.ReadAsStream()).ReadToEnd());
        
        
        // context.Response.Redirect("https://localhost:7260");
        
        return;
    }
    await next.Invoke();
});

app.UseAuthentication();
app.UseAuthorization();

// var db = app.Services.CreateScope().ServiceProvider.GetService<DatabaseContext>();
// db.Database.EnsureCreated();

app.Run();
