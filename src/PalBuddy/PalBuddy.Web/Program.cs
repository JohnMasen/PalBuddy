using Microsoft.Extensions.Localization;
using PalBuddy.Core;
using PalBuddy.Web.Components;

namespace PalBuddy.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services
                .AddLocalization()
                .AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services.AddSingleton(new PalDedicatedServer(builder.Configuration["ServerPath"]));
                var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
            }
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();


            var supportedCultures = new[] { "en-US", "zh-CN" };
            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            app.UseRequestLocalization(localizationOptions);    

            app.Run();
        }
    }
}
