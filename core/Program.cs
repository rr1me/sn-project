using System.Text.Json.Serialization;
using core;
using core.Authentication;
using core.Data;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(x =>
{
    x.AddFile("app.log", x=>x.MinLevel = LogLevel.Debug);
});

builder.Services.AddDbContext<DatabaseContext>(x =>
{
    var connectionString = builder.Configuration.GetSection("ConnectionString").Value;
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    x.UseMySql(connectionString, serverVersion);
});

builder.Services.AddSingleton<GatewayAuthenticationHandler>();
builder.Services.AddSingleton<JwtHandler>();

builder.Services.AddScoped<InternalControls>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "GatewayAuthScheme";
}).AddScheme<GatewayAuthScheme, AuthenticationMiddleware>("GatewayAuthScheme", null);
builder.Services.AddAuthorization(
    x =>
    {
        x.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
        x.AddPolicy("User", policy => policy.RequireRole("User"));
    }
);

builder.Services.AddControllers().AddJsonOptions(x =>
{
    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddHttpClient();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseRouting();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

// var scope = app.Services.CreateScope();
// var ctrl = scope.ServiceProvider.GetRequiredService<InternalControls>();
// ctrl.Initialize();

app.Use(async (context, next) =>
{
    var logger = app.Logger;

    var requestPath = context.Request.Path;
    var requestMethod = context.Request.Method;
    var remoteIp = context.Connection.RemoteIpAddress.ToString();
    logger.LogInformation($"Starting: {requestPath} | Method: {requestMethod} | RemoteIp: {remoteIp} _______________________________________________________________");
    logger.LogInformation($"Headers: {string.Join(" | ", context.Request.Headers.ToArray())} _______________________________________________________________");
    
    await next.Invoke();

    var responseStatusCode = context.Response.StatusCode;
    logger.LogInformation($"Ending: {requestPath} | Code: {responseStatusCode} __________________________________________________________________" );
});

app.Run();
