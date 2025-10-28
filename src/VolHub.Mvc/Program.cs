using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using VolHub.Mvc.Data;
using VolHub.Mvc.Models;
using VolHub.Mvc.Security;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// MVC + localization
builder.Services.AddControllersWithViews();
builder.Services.AddLocalization();

var supportedCultures = new[]
{
    new CultureInfo("uk-UA"),
    new CultureInfo("ru-RU"),
    new CultureInfo("en-US")
};

// Auth0 (OIDC)
var domain = builder.Configuration["Auth0:Domain"]!;
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddCookie()
.AddOpenIdConnect(options =>
{
    options.Authority = $"https://{domain}";
    options.ClientId = builder.Configuration["Auth0:ClientId"];
    options.ClientSecret = builder.Configuration["Auth0:ClientSecret"];
    options.ResponseType = "code";
    options.SaveTokens = true;
    options.CallbackPath = "/signin-oidc";
    options.Scope.Add("openid");
    options.Scope.Add("profile");
    options.Scope.Add("email");

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async ctx =>
        {
            var email = ctx.Principal?.FindFirst(ClaimTypes.Email)?.Value
                        ?? ctx.Principal?.FindFirst("email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                return;
            }

            var db = ctx.HttpContext.RequestServices.GetRequiredService<AppDbContext>();
            var exists = await db.Profiles.AnyAsync(p => p.Email == email);
            if (exists)
            {
                return;
            }

            db.Profiles.Add(new AppUserProfile
            {
                Email = email,
                Username = email.Split('@')[0],
                FullName = string.Empty,
                Phone = "+380000000000",
                PasswordHash = PasswordHelper.HashPassword(Guid.NewGuid().ToString("N"))
            });

            await db.SaveChangesAsync();
        }
    };
});

var app = builder.Build();

app.UseHttpsRedirection();

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("uk-UA"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
