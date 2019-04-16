using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Logging;

using Angelo.Common.Extensions;
using Angelo.Plugins.Internal;

namespace Angelo.Plugins
{
    public class PluginProvider
    {
        private ILogger<PluginProvider> _logger;
        private PluginBuilder _pluginBuilder;
        private PluginContext _pluginContext;
        private PluginConfig _pluginConfig;
        private PluginFileProvider _pluginFileProvider;
        private IEnumerable<Assembly> _assemblies;

        public static PluginProvider Startup(Action<PluginConfig> options)
        {
            var config = new PluginConfig();

            options.Invoke(config);

            return new PluginProvider(config);
        }

        public PluginProvider(PluginConfig config)
        {
            _logger = config.LoggerFactory.CreateLogger<PluginProvider>();

            _logger.LogInformation("Initializing Plugin Context");     
            _pluginContext = new PluginContext(config);
            _pluginConfig = config;

            _logger.LogInformation("Scanning Assemblies for Plugins");
            _pluginContext.Assemblies = GetAssemblies(config.AssemblyFolder);
            _pluginContext.Plugins = _pluginContext.Assemblies.GetInstances<IPlugin>();

            _pluginBuilder = new PluginBuilder(_pluginContext);
            _pluginFileProvider = new PluginFileProvider(_pluginContext);

            foreach (var plugin in _pluginContext.Plugins)
            {
                _logger.LogInformation("Startup for Plugin '{0}'", plugin.Name);
                plugin.Startup(_pluginBuilder);
            }
        }
      
        public void ConfigureServices(IServiceCollection services)
        {
            var mvcBuilder = services.AddMvc();
           
            foreach (Assembly assembly in _pluginContext.Assemblies)
                mvcBuilder.AddApplicationPart(assembly);

            _logger.LogInformation("Adding File Provider for Embedded Razor Views");

            mvcBuilder.AddRazorOptions(options => {
                foreach (Assembly assembly in _pluginContext.Assemblies)
                {
                    options.FileProviders.Add(new EmbeddedFileProvider(assembly, assembly.GetName().Name));
                }
            });

            _logger.LogInformation("Adding Core Services");

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(_pluginContext);
            services.AddSingleton(_pluginBuilder);
            services.AddSingleton(_pluginFileProvider);

            _logger.LogInformation("Applying Service Configurations");

            foreach (var serviceConfig in _pluginContext.ServiceConfigurations)
            {
                serviceConfig.Invoke(services);
            }
        }

        public void Configure(IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            _logger.LogInformation("Adding File Provider for Embedded Static Files");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new CompositeFileProvider(_pluginFileProvider)
            });

            _logger.LogInformation("Adding Route Configurations");
            app.UseMvc(routes => {
                foreach (var routeConfig in _pluginContext.RouteConfigurations )
                {
                    routeConfig.Invoke(routes);
                }
            });

            _logger.LogInformation("Applying App Configurations");
            foreach(var appConfig in _pluginContext.AppConfigurations)
            {
                appConfig.Invoke(app);
            }

        }
 
        public void RunStartupActions(IApplicationBuilder app)
        {
            var services = app.ApplicationServices;

            _logger.LogInformation("Invoking Plugin Startup Actions");
            foreach (var action in services.GetServices<IPluginStartupAction>())
            {
                action.Invoke();
            }
        }

        private IEnumerable<Assembly> GetAssemblies(string pluginFolder)
        {
            var rootPath = _pluginConfig.HostingEnvironment.ContentRootPath;

            if (pluginFolder.StartsWith("\\"))
                pluginFolder = pluginFolder.Substring(1);

            pluginFolder = System.IO.Path.Combine(rootPath, pluginFolder ?? "");

            if (!System.IO.Directory.Exists(pluginFolder))
            {
                System.IO.Directory.CreateDirectory(pluginFolder);
            }


            var assemblyProvider = new AssemblyProvider(pluginFolder, _pluginConfig.LoggerFactory);


            return assemblyProvider.GetAssemblies();
        }
    }
}