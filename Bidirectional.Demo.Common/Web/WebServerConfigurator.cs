using System;
using System.Linq;
using Bidirectional.Demo.Common.DependencyInjection;
using Microsoft.AspNetCore.HostFiltering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bidirectional.Demo.Common.Web
{
    public class WebServerConfigurator : IConfigurator
    {
        public void Configure(HostBuilderContext context, IServiceCollection services)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            /* Web server */
            var webServerSettingsConfigurationSection = context.Configuration.GetSection(nameof(WebServerSettings));
            var webServerSettings = webServerSettingsConfigurationSection.Get<WebServerSettings>();

            var webServerSettingsValidationErrors = webServerSettings.Validate().ToList();
            if (webServerSettingsValidationErrors.Any())
            {
                throw new WebServerSettingsException(string.Join(Environment.NewLine, webServerSettingsValidationErrors));
            }

            services.AddOptions().Configure<WebServerSettings>(webServerSettingsConfigurationSection);
            services.AddSingleton<IConfigureOptions<HostFilteringOptions>, HostFilteringOptionsConfigurator>();
            services.AddSingleton<IConfigureOptions<KestrelServerOptions>, KestrelOptionsConfigurator>();
        }
    }
    
    public class HostFilteringOptionsConfigurator: IConfigureOptions<HostFilteringOptions>
    {
        private readonly WebServerSettings _webServerSettings;
        private readonly ILogger<HostFilteringOptionsConfigurator> _logger;

        public HostFilteringOptionsConfigurator(IOptions<WebServerSettings> webServerSettings, ILogger<HostFilteringOptionsConfigurator> logger)
        {
            _webServerSettings = webServerSettings?.Value ?? throw new ArgumentNullException(nameof(webServerSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void Configure(HostFilteringOptions options)
        {
            var allowedHosts = _webServerSettings.AllowedHosts!.ToList();

            // The Health Checks module requires that 'localhost' is an allowed host name
            if (!allowedHosts.Contains("localhost", StringComparer.OrdinalIgnoreCase))
            {
                allowedHosts.Add("localhost");
            }
            
            _logger.LogInformation("Configuring web server to allow the following hosts: {AllowedHosts}", (object) allowedHosts);
            
            options.AllowedHosts = allowedHosts.ToList();
        }
    }
    
    public class KestrelOptionsConfigurator: IConfigureOptions<KestrelServerOptions>
    {
        private readonly WebServerSettings _webServerSettings;
        private readonly ILogger<KestrelOptionsConfigurator> _logger;

        public KestrelOptionsConfigurator(IOptions<WebServerSettings> webServerSettings, ILogger<KestrelOptionsConfigurator> logger)
        {
            _webServerSettings = webServerSettings?.Value ?? throw new ArgumentNullException(nameof(webServerSettings));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public void Configure(KestrelServerOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            var httpSettings = _webServerSettings.Http;
            var httpsSettings = _webServerSettings.Https;
            
            /* HTTP */
            if (httpSettings?.IsEnabled == true)
            {
                var httpIPAddress = httpSettings!.IPAddress!.ToIpAddress();
                var httpPort = httpSettings!.Port!.Value;
                
                _logger.LogDebug("Configuring HTTP to listen on {HttpIPAddress:l}:{HttpPort}",  httpIPAddress, httpPort);

                options.Listen(httpIPAddress, httpPort);
            }
            
            /* HTTPS */
            if (httpsSettings?.IsEnabled == true)
            {
                var httpsIPAddress = httpsSettings!.IPAddress!.ToIpAddress();
                var httpsPort = httpsSettings!.Port!.Value;
                var httpsCertificate = httpsSettings!.Certificate!.FindCertificate();
                
                _logger.LogDebug("Configuring HTTPS to listen on {HttpsIPAddress:l}:{HttpsPort} " +
                                 "using certificate with subject {Subject}, issuer {Issuer} and thumbprint {Thumbprint}",
                    httpsIPAddress, httpsPort, httpsCertificate!.SubjectName.Name, httpsCertificate.IssuerName.Name, httpsCertificate.Thumbprint);

                options.Listen(httpsIPAddress, httpsPort, listenOptions =>
                {
                    listenOptions.UseHttps(httpsOptions =>
                    {
                        httpsOptions.ServerCertificate = httpsCertificate;
                    });
                });
            }
        }
    }
}