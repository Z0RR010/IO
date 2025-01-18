using IO.Components;
using IO.Modules.Communication;
using IO.Modules.MapLibrary;
using Microsoft.JSInterop;
using IO.Modules.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.Cookies;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.UI.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace IO
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = "auth_token";
                    options.LoginPath = "/signIn";
                    options.Cookie.MaxAge = TimeSpan.FromMinutes(60);
                });
            builder.Services.AddAuthorization();
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite("Data Source=./databases/userDatabase.db"));
            builder.Services.AddControllers();

            /*
            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<UserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, PersistingRevalidatingAuthenticationStateProvider>();
            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme).AddIdentityCookies();
            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();
            */

            builder.Services.AddSingleton<Communicator>();
            builder.Services.AddSingleton(provider =>
            {
                var communicator = provider.GetRequiredService<Communicator>();
                return communicator.manager; // Use the same instance from Communicator
            });
            builder.Services.AddHttpClient();
            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddScoped(sp => new HttpClient
                {
                    BaseAddress = new Uri("http://localhost:5236")
                });
            }
            else
            {
                builder.Services.AddScoped(sp => new HttpClient
                {
                    BaseAddress = new Uri("http://ioserver.ddns.net")
                });
            }


            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });


            builder.Services.AddScoped<GoogleMapsClient>(provider =>
            {
                string apiKey = "AIzaSyCaEHkCZC5zP2OjibM8Ri2I7D-1UoZLU8M";
                var jsRuntime = provider.GetRequiredService<IJSRuntime>();

                return new GoogleMapsClient(jsRuntime, apiKey);
            });

            builder.Services.AddScoped<RequestModule.IRequestService, RequestModule.RequestService>();
            builder.Services.AddLocalization();
            var supportedCultures = new[] { "en-US", "pl" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            builder.Services.AddScoped<SessionService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();
            app.UseAntiforgery();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRequestLocalization(localizationOptions);
            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.MapControllers();

            app.Run();
        }
    }
}
