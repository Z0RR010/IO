using IO.Components;
using IO.Modules.MapLibrary;
using Microsoft.JSInterop;

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

            builder.Services.AddControllers();

            builder.Services.AddHttpClient();
            builder.Services.AddScoped(sp => new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5236")
            });

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

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();
            app.MapControllers();

            app.Run();
        }
    }
}
