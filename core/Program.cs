using core.IdentityConfig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var idConfig = new IdentityConfig();

// builder.Services.AddAuthentication("Bearer")
//     .AddJwtBearer("Bearer", options =>
//     {
//         options.Authority = "https://localhost:7274";
//
//         options.TokenValidationParameters = new TokenValidationParameters
//         {
//             ValidateAudience = false
//         };
//     });

// builder.Services.AddAuthorization(x => x.FallbackPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

builder.Services.AddIdentityServer()
    .AddDeveloperSigningCredential()
    .AddInMemoryIdentityResources(idConfig.IdentityResources)
    .AddInMemoryApiResources(idConfig.ApiResources)
    .AddInMemoryApiScopes(idConfig.ApiScopes)
    .AddInMemoryClients(idConfig.Clients)
    .AddJwtBearerClientAuthentication();

builder.Services.AddLocalApiAuthentication();

// builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
//     .AddCookie();

var app = builder.Build();

// app.UseHttpsRedirection();

app.UseRouting();

// app.UseAuthentication();
// app.UseAuthorization();
            
app.UseIdentityServer();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// app.UseCookiePolicy(new CookiePolicyOptions { MinimumSameSitePolicy = SameSiteMode.Lax });

app.Run();