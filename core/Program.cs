using System.Text.Json.Serialization;
using core.Authentication;
using core.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DatabaseContext>(x =>
{
    var connectionString = "server=localhost;user=root;password=root;database=sntestdb";
    var serverVersion = ServerVersion.AutoDetect(connectionString);
    x.UseMySql(connectionString, serverVersion);
}
    // , ServiceLifetime.Singleton
    );

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

// var db = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope().ServiceProvider.GetRequiredService<DatabaseContext>();
// db.Database.EnsureCreated();
// var user = new UserEntity("2nduser", BCrypt.Net.BCrypt.HashPassword("321"), Roles.User);
// db.Users.Add(user);
// db.SaveChanges();

app.Run();
