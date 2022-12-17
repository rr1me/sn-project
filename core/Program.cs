using core.IdentityConfig;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var idConfig = new IdentityConfig();

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryApiScopes(idConfig.ApiScopes)
    .AddInMemoryClients(idConfig.Clients);

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseRouting();
app.UseIdentityServer();

app.MapControllers();

app.Run();