using System.Text.Json.Serialization;
using core.Authentication;
using core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(x =>
{
    const string connectionString = "server=localhost;user=root;password=root;database=sntestdb";
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    x.UseMySql(connectionString, serverVersion);
});

builder.Services.AddSingleton<GatewayAuthenticationHandler>();
builder.Services.AddSingleton<JwtHandler>();

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

app.UseRouting();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
