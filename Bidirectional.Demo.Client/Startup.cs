using System;
using Bidirectional.Demo.Common.Extensions;
using Bidirectional.Demo.Common.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bidirectional.Demo.Client
{
    public class Startup
    {
        public IConfigurationRoot Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = (IConfigurationRoot) configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            
            services.AddControllersWithViews()
                .AddApplicationPart(typeof(Startup).Assembly);

            services.AddHttpContextAccessor();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (env == null)
            {
                throw new ArgumentNullException(nameof(env));
            }

            var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
            var webServerSettings = app.ApplicationServices.GetRequiredService<IOptions<WebServerSettings>>().Value;
            
            logger.LogInformation($"Configuring web server for environment '{env.EnvironmentName}'");
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }
            
            app.UseStaticFiles();
            
            app.UseRouting();
            
            app.UseCors();
            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });

            logger.LogInformation("Web server is up and running.");

            if (webServerSettings.Http != null && webServerSettings.Http.IsEnabled)
            {
                logger.LogInformation("Listening on HTTP : {IPAddress:l}:{Port}", webServerSettings.Http.IPAddress.ToIpAddress(), webServerSettings.Http.Port);
            }
            if (webServerSettings.Https != null && webServerSettings.Https.IsEnabled)
            {
                logger.LogInformation("Listening on HTTPS: {IPAddress:l}:{Port}", webServerSettings.Https.IPAddress.ToIpAddress(), webServerSettings.Https.Port);
            }
        }
    }
}