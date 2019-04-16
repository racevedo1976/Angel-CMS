using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using IdentityModel;
using IdentityServer4.Services;
using IdentityServer4.Validation;
using IdentityServer4.Configuration;

using Angelo.Aegis.Configuration;
using Angelo.Aegis.Internal;
using Angelo.Aegis.Messaging;
using Angelo.Common.Logging;
using Angelo.Common.Mvc;

using Angelo.Identity;
using Angelo.Identity.Models;

namespace Angelo.Aegis
{
    public class Startup
    {
        private IHostingEnvironment _environment;
        private IConfigurationRoot _configuration;

        public Startup(IHostingEnvironment environment)
        {        
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("settings.json")
                .AddJsonFile($"settings.{environment.EnvironmentName}.json", optional: true);

            _environment = environment;
            _configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {

            // configuring options
            ServerOptions serverOptions;
            CultureOptions cultureOptions;

            services.Configure<ServerOptions>(_configuration);
            services.Configure<SmtpOptions>(_configuration.GetSection("Smtp"));
            services.Configure<CultureOptions>(_configuration.GetSection("CultureOptions"));
            services.Configure<LdapOptions>(_configuration.GetSection("LdapOptions"));
            services.AddOptions();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            serverOptions = serviceProvider.GetService<IOptions<ServerOptions>>().Value;
            cultureOptions = serviceProvider.GetService<IOptions<CultureOptions>>().Value;

            // localization settings
            services.Configure<RequestLocalizationOptions>(
                options => cultureOptions.SetRequestOptions(options)
            );

            services.AddLocalization(options =>
                options.ResourcesPath = cultureOptions.ResourcePath
            );

            // SAAS
            services.AddMultitenancy<AegisTenant, AegisTenantResolver>();
            services.AddScoped<AegisTenantResolver>();

            // Identity Server          
            var rsa = RSA.Create();
            var key = new RsaSecurityKey(rsa) { KeyId = "dfb409f59f0c3d6b21e117a96339c887" };
            var credential = new SigningCredentials(key, "RS256");

            services
                .AddIdentityServer()
                .AddInMemoryPersistedGrants()
                .AddInMemoryIdentityResources(Resources.GetIdentityResources())
                .AddInMemoryApiResources(Resources.GetApiResources())
                .AddClientStore<AegisTenantClientStore>()
                //.AddInMemoryClients(Clients.Get())
                .AddSigningCredential(credential);

            // Custom per tenant options classes
            services.AddScoped<IdentityServerOptions, AegisTenantIdentityServerOptions>();
            services.AddSingleton<IOptions<IdentityOptions>, IdentityOptionsFactory>();

            // Backed by AspnetCore.Identity 
            services.AddAngeloIdentity();

            //AddLdapServices
            services.AddLdapServices();

            // IdentityServer Adapters for AspNetCore.Identy
            services.AddTransient<IResourceOwnerPasswordValidator, IdentityPasswordValidator>();
            services.AddTransient<IProfileService, IdentityProfileService>();

            services.AddDbContext<IdentityDbContext>(options => {
                options.UseSqlServer(
                    serverOptions.Data.ConnectionString,
                    settings =>
                    {
                        settings.MigrationsAssembly("Angelo.Connect.Web");
                        settings.UseRowNumberForPaging();
                    }
                );
            });

            // Other Services
            services.AddTransient<EmailProvider>();
            services.AddTransient<MessagingService>();
            services.AddTransient<TemplateService>();
            services.AddTransient<ICypher, Base64Cypher>();

            // Startup Extensions
            services.AddAutoMapperMappings();

            // MVC
            services
                .AddMvc(config => {
                    // Allow XML Content Negotiation
                    config.RespectBrowserAcceptHeader = true;
                    config.InputFormatters.Add(new XmlSerializerInputFormatter());
                    config.OutputFormatters.Add(new XmlSerializerOutputFormatter());
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization()
                .AddRazorOptions(razor => {
                    razor.ViewLocationExpanders.Add(new UI.CustomViewLocationExpander());
                });
               
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory, IHostingEnvironment hostingEnvironment, IOptions<ServerOptions> serverOptions, IOptions<RequestLocalizationOptions> l10nOptions)
        {
            loggerFactory.AddConsole(LogLevel.Debug);
            loggerFactory.AddDebug(LogLevel.Debug);
            
            /* 
            // This works, but was for testing purposes only. Disabling for now.
            loggerFactory.AddProvider(FileLoggerProvider.Create( x =>
            {
                x.IncludeConsole = true;
                x.FilePath = @".\efcore.log";
                x.Filters = new string[] { "Microsoft.EntityFrameworkCore.Storage.DbCommandLogData" };
            }));
            */

            if (!hostingEnvironment.IsProduction())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseMultitenancy<AegisTenant>();

            // Multi-tenant aware Idsvr is mounted on "/auth/<tenant>/" via this construction
            app.Map("/auth", multiTenantIdsvrMountPoint => {

                // Angelo.Common.Mvc.Saas 

                multiTenantIdsvrMountPoint.UseMultitenancy<AegisTenant>();
                multiTenantIdsvrMountPoint.UsePerTenant<AegisTenant>((ctx, builder) => {

                    var tenant = ctx.Tenant;
                    var mountPath = "/" + tenant.TenantKey;

                    // we mount the tenant specific IdSvr4 app under /tenants/<tenant>/
                    builder.Map(mountPath, perTenantApp => {

                        // Configure Auth cookie used by AspNetIdentity / IdentityServer
                        perTenantApp.UseCookieAuthentication(new CookieAuthenticationOptions()
                        {
                            AuthenticationScheme = tenant.AuthSchemeInternal,
                            CookieName = tenant.AuthSchemeInternal,
                            SlidingExpiration = tenant.CookieSlidingExpiration,
                            ExpireTimeSpan = tenant.CookieLifetime,
                            AutomaticAuthenticate = true,
                            AutomaticChallenge = true
                        });

                        // Configure Auth cookie for exterhnal providers (if needed)
                        if (tenant.ProviderOptions?.Google != null)
                        {
                            perTenantApp.UseCookieAuthentication(new CookieAuthenticationOptions()
                            {
                                AuthenticationScheme = tenant.AuthSchemeExternal,
                                CookieName = tenant.AuthSchemeExternal,
                                SlidingExpiration = tenant.CookieSlidingExpiration,
                                ExpireTimeSpan = tenant.CookieLifetime,
                            });

                            perTenantApp.UseGoogleAuthentication(new GoogleOptions()
                            {
                                ClientId = tenant.ProviderOptions.Google.ClientId,
                                ClientSecret = tenant.ProviderOptions.Google.ClientSecret,
                                AuthenticationScheme = tenant.ProviderOptions.Google.AuthScheme,
                                SignInScheme = tenant.AuthSchemeExternal
                            });
                        }

                        perTenantApp.UseIdentityServer();
                        perTenantApp.UseMvcWithDefaultRoute();
                        perTenantApp.UseRequestLocalization(l10nOptions.Value);

                        /*
                        perTenantApp.UseMvc(routes => {
                            routes.MapRoute(name: "Account",
                                template: "account/{action=Login}",
                                defaults: new { controller = "Account" });
                        });

                        perTenantApp.UseMvc(routes => {
                            routes.MapRoute(name: "Consent",
                                template: "consent/{action=Index}",
                                defaults: new { controller = "Consent" });
                        });

                        perTenantApp.UseMvc(routes => {
                            routes.MapRoute(name: "Manage",
                                template: "manage/{action=Index}",
                                defaults: new { controller = "Manage" });
                        });
                        */
                    });
                });
            });

            // Configure for non "/auth/<tenant>" routes
            app.UseStaticFiles();
            app.UseRequestLocalization(l10nOptions.Value);

            app.UseMvc(routes => {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
