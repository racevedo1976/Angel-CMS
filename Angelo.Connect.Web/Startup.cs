using System;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Angelo.Common.Migrations;
using Angelo.Common.Mvc.Services;
using Angelo.Common.Extensions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Extensions;
using Angelo.Connect.Logging;
using Angelo.Common.Messaging;
using Angelo.Connect.Rendering.Filters;
using Angelo.Connect.Services;
using Angelo.Connect.UserConsole;
using Angelo.Connect.Web.Jobs;
using Angelo.Connect.Web.Extensions;
using Angelo.Connect.Web.UI.UserConsole.Components;
using Angelo.Identity;
using Angelo.Jobs;
using Angelo.Plugins;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;


namespace Angelo.Connect.Web
{
    public class Startup
    {
        
        private IConfigurationRoot _configuration;
        private IHostingEnvironment _environment;
        private ILoggerFactory _loggerFactory;
        private PluginProvider _plugins;

        public Startup(IHostingEnvironment environment, ILoggerFactory loggerFactory)
        {
            // Set up configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(environment.ContentRootPath)
                .AddJsonFile("settings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"settings.{environment.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();

            _environment = environment;
            _loggerFactory = loggerFactory;
            _configuration = builder.Build();

            // TODO: Move to Plugin Initialization
           

            // Startup Plugin Framework
            _plugins = PluginProvider.Startup(config => {
                config.HostingEnvironment = environment;
                config.ConfigurationRoot = _configuration;
                config.LoggerFactory = _loggerFactory;
                config.AssemblyFolder = _configuration.GetValue<string>("PluginFolder");
                config.ConnectionString = _configuration.GetConnectionString("ConnectDB");
            });
        }

        // This method gets called by the runtime. 
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuring Options
            ServerOptions serverOptions;
            AegisOptions aegisOptions;
            DriveOptions driveOptions;


            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<IConfigurationRoot>(x => _configuration);
            services.Configure<ServerOptions>(_configuration.GetSection("WebServer"));
            services.Configure<SiteOptions>(_configuration.GetSection("SiteOptions"));
            services.Configure<AegisOptions>(_configuration.GetSection("OpenIdConnect"));
            services.Configure<DriveOptions>(_configuration.GetSection("Drive"));
            services.Configure<SmtpOptions>(_configuration.GetSection("Smtp"));
            services.AddOptions();

            IServiceProvider serviceProvider = services.BuildServiceProvider();
            serverOptions = serviceProvider.GetService<IOptions<ServerOptions>>().Value;
            aegisOptions = serviceProvider.GetService<IOptions<AegisOptions>>().Value;
            driveOptions = serviceProvider.GetService<IOptions<DriveOptions>>().Value;

            // Localization
            services.Configure<RequestLocalizationOptions>(
                localizationOptions => serverOptions.SetRequestLocatizationOptions(localizationOptions)
            );

            services.AddLocalization(options =>
                options.ResourcesPath = serverOptions.ResourcePath
            );

            // Db Contexts
            services.AddDbContext<ConnectDbContext>(options => {
                options.UseSqlServer(
                    _configuration.GetConnectionString("ConnectDB"),
                    settings =>
                    {
                        settings.MigrationsAssembly("Angelo.Connect.Web");
                        settings.MigrationsHistoryTable("MigrationsEF", "app");
                        settings.UseRowNumberForPaging();
                    }
                );
            
            });
            services.AddDbContext<IdentityDbContext>(options => {
                options.UseSqlServer(
                    _configuration.GetConnectionString("IdentityDB"),
                     settings =>
                     {
                         settings.MigrationsAssembly("Angelo.Connect.Web");
                         settings.MigrationsHistoryTable("MigrationsEF", "app");
                         settings.UseRowNumberForPaging();
                     }
                );
            });
            services.AddDbContext<DbLogContext>(options => {
                options.UseSqlServer(
                    _configuration.GetConnectionString("ConnectDB"),
                     settings =>
                     {
                         settings.MigrationsAssembly("Angelo.Connect.Web");
                         settings.MigrationsHistoryTable("MigrationsEF", "app");
                         settings.UseRowNumberForPaging();
                     }
                );
            });
            services.AddDistributedSqlServerCache(options => {
                options.ConnectionString = _configuration.GetConnectionString("ConnectDB");
                options.SchemaName = "app";
                options.TableName = "WebCache";
            });

            // Migrations
            services.AddAppMigrationRepo(_configuration.GetConnectionString("ConnectDB"));
            services.AddCoreMigrations();

            // Core Services             
            services.AddAngeloIdentity();
            services.AddConnectCore(options => {
                options.FileSystemRoot = _environment.ContentRootPath;
                options.UseHttpsForAbsoluteUris = serverOptions.UseHttpsRoutes;
                options.SupportedCultures = serverOptions.GetSupportedCultures();
                options.DefaultCulture = serverOptions.GetDefaultCulture();
                options.AegisAuthority = aegisOptions.Authority;
                options.ConnectConnectionString = _configuration.GetConnectionString("ConnectDB");
                options.IdentityConnectionString = _configuration.GetConnectionString("IdentityDB");
                options.NotifyMeUnsubscribeDefaultDomain = _configuration["NotifyMe:UnsubscribeDefaultDomain"];
                options.NotifyMeUnsubscribePath = _configuration["NotifyMe:UnsubscribePath"];
                options.EmailServerHost = _configuration.GetValue("NotifyMe:EmailServerHost", "antispam.MyCompany.org");
                options.EmailServerPort = _configuration.GetValue("NotifyMe:EmailServerPort", 25);
                options.TemplatesPath = _configuration.GetValue<string>("TemplatesDirectory");
                options.TemplateExportPath = _configuration.GetValue<string>("TemplatesDirectory");
                options.SearchIndexRoot = _configuration.GetValue<string>("SearchIndexRoot");
            });
            services.AddConnectDocuments(options => {
                options.DriveAuthority = driveOptions.Authority;
            });
            services.AddAutoMapperMappings();
            services.AddLdapServices();
          
            // Context Providers (Site, Page, Menu)
            services.AddUserContext<UserContextAccessor>();
            services.AddAdminContext<AdminContextAccessor>();
            services.AddClientAdminContext<ClientAdminContextAccessor>();
            services.AddSiteAdminContext<SiteAdminContextAccessor>();
            services.AddSiteContext<SiteContextAccessor>();
            services.AddMultitenancy<SiteContext, SiteContextResolver>();

            // Core menus
            services.AddConnectCoreMenus();

            // Jobs
            services.AddJobs(options => {
                options.UseSqlServer(db => {
                    db.Schema = "app";
                    db.KeyStoreTable = "Meta";
                    db.ConnectionString = _configuration.GetConnectionString("ConnectDB");
                });
                options.UseCronJobRegistry<JobRegistry>();
            });


            // UserConsole Components
            services.AddTransient<IUserConsoleCustomComponent, UI.UserConsole.Components.UserMessageComponent>();
            services.AddTransient<IUserConsoleTreeComponent, UI.UserConsole.Components.UserLibraryComponent>();
            services.AddTransient<IUserConsoleTreeComponent, UI.UserConsole.Components.UserPageComponent>();
            services.AddTransient<UI.UserConsole.Components.UserPageComponent>();
            services.AddTransient<IUserConsoleCustomComponent, SiteAlertsComponents>();

            //services.AddTransient<JobCountUsers>();
            //services.AddTransient<JobLongRunning>();
            services.AddTransient<JobImportLdapUsers>();
            services.AddTransient<JobProcessNotifications>();

            // 3rd party libraries
            services.AddKendo();

            // add messaging
            services.AddTransient<EmailProvider>();
            services.AddTransient<TemplateEmailService>();
            services.AddTransient<TemplateService>();
            services.AddTransient<SiteAlertsManager>();

            // Add Authorization Policies
            services.AddSecurityPolicies();
            services.AddClaimsSecurity();

            //Auth Event Handlers
            services.AddTransient<OpenIdInterceptors>();
            services.AddTransient<CookieAuthInterceptors>();

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.BufferBodyLengthLimit = long.MaxValue;
                options.BufferBody = true;
            });

            // MVC / Hosting          
            services.AddMvc()
                .AddMvcOptions(options => {
                    options.Filters.Add(typeof(ReturnUrlActionFilter));

                    // Require HTTPS requests if the HTTPS rewriter is enabled
                    if (serverOptions.UseHttpsRewriter)
                    {
                        options.Filters.Add(new Microsoft.AspNetCore.Mvc.RequireHttpsAttribute());
                    }
                })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options => {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                        factory.Create(typeof(Global));
                })
                .AddJsonOptions(options => {
                    // add default json formatting instead of camelcase due to Telerik
                    options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.DefaultContractResolver();
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
                });

            services.Configure<RazorViewEngineOptions>(options => {
                options.ViewLocationExpanders.Add(new ViewLocationExpander());
            });

            services.AddSession(options => {
                options.CookiePath = "/";
                options.IdleTimeout = new TimeSpan(0, serverOptions.SessionMinutes, 0);
            });

            // Plugins
            _plugins.ConfigureServices(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IOptions<AegisOptions> aegisOptions, IOptions<ServerOptions> serverOptions, IOptions<RequestLocalizationOptions> l10nOptions, DbLoggerProvider dbLogProvider)
        {
            if (_environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/sys/error");
            }

            // Https Rewriter 
            if(serverOptions.Value.UseHttpsRewriter)
            {
                var rewriteOptions = new RewriteOptions().AddRedirectToHttps();
                app.UseRewriter(rewriteOptions);
            }

            // logging
            //_loggerFactory.AddProvider(dbLogProvider);
            _loggerFactory.AddConsole(_configuration.GetSection("Logging"));
            _loggerFactory.AddDebug(LogLevel.Debug);
          

            // app components
            app.UseSession();
            app.UseMultitenancy<SiteContext>();
            app.UseRequestLocalization(l10nOptions.Value);
            app.UseStaticFiles();
            app.UseKendo(_environment);

            // cookie policy
            app.UseCookiePolicy(new CookiePolicyOptions
            {
                HttpOnly = HttpOnlyPolicy.None,
                Secure = CookieSecurePolicy.SameAsRequest
            });

            // openid security      
            app.UsePerTenant<SiteContext>((ctx, builder) => {
                builder.UseAegisOidc(aegisOptions, ctx.Tenant);
            });

            // Application migrations
            app.RunAppMigrations();

            //validate Client Active Status
            //app.RunActiveClientValidation();

            // configure plugins
            _plugins.Configure(app);

            // configure MVC convention based routes
            app.UseMvc(routes => {

                #region Ajax / Api Services

                //TODO: Cleanup Widget routes. Move behind sys/
                routes.MapRoute(
                  name: "admin",
                  template: "admin/{controller=Dashboard}/{action=Index}/{id?}",
                  defaults: new { area = "Admin" }
                );

                //TODO: route behind "sys/"
                routes.MapRoute(
                    name: "Ajax Components",
                    template: "components/{type}/{cid?}",
                    defaults: new { controller = "Components", action = "Render" }
                );

                routes.MapRoute(
                   name: "content",
                   template: "sys/content/{action}",
                   defaults: new { controller = "Content" }
               );
                #endregion

                #region Back Office Routes 

                //TODO: use "sys/account" instead
                routes.MapRoute(
                    name: "User Account",
                    template: "sys/account/{action=profile}",
                    defaults: new { area = "Admin", controller = "Account" }
                );

                routes.MapRoute(
                    name: "Corp Admin",
                    template: "sys/corp/admin/{action=dashboard}",
                    defaults: new { area = "Admin", controller = "CorpAdmin" }
                );

                routes.MapRoute(
                    name: "Client Admin",
                    template: "sys/clients/{tenant}/admin/{action=dashboard}",
                    defaults: new { area = "Admin", controller = "ClientAdmin" }
                );

                routes.MapRoute(
                    name: "Site Admin",
                    template: "sys/sites/{tenant}/admin/{action=dashboard}",
                    defaults: new { area = "Admin", controller = "SiteAdmin" }
                );

                #endregion

                #region CMS Front End Routes

                routes.MapRoute(
                    name: "Notify Me",
                    template: "sys/notifyme/{action}",
                    defaults: new { area = "Public", controller = "NotifyMeSignUp" }
                );
                //routes.MapRoute(
                //    name: "Site Alerts",
                //    template: "sys/sitealerts/{action}",
                //    defaults: new { area = "Public", controller = "SiteAlerts" }
                //);
                routes.MapRoute(
                    name: "Search",
                    template: "sys/search/{action}",
                    defaults: new { area = "Public", controller = "Search" }
                );
                routes.MapRoute(
                    name: "Page Provider by Route",
                    template: "{*route}",
                    defaults: new { area = "Public", controller = "SitePage", action = "RenderRoute", route = "/" }
                );
                #endregion

            });

            // enable jobs
            app.UseJobs();

            //-----------------------------------------------------------
            // TODO: These need to go away - Convert to IAppMigrations 
            // -----------------------------------------------------------
            app.RunStartupActions();
            _plugins.RunStartupActions(app);
        }
    }

}
