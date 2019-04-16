using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;


namespace Angelo.Plugins
{
    public class PluginBuilder
    {
        public PluginContext PluginContext { get; }
       
        public PluginBuilder(PluginContext pluginContext)
        {
            PluginContext = pluginContext;
        }

        public void AddDbContext<TDbContext>() where TDbContext : DbContext
        {
            AddDbContext<TDbContext>(null);
        }

        public void AddDbContext<TDbContext>(Action<TDbContext> startupAction) where TDbContext : DbContext
        {
            // register dbcontext with service provider
            PluginContext.ServiceConfigurations.Add(services => {
                services.AddDbContext<TDbContext>(options => {
                    options.UseSqlServer(PluginContext.Config.ConnectionString,
                        settings =>
                        {
                            settings.UseRowNumberForPaging();
                        }
                    );
                });
            });

            // register startup action
            if(startupAction != null)
            {
                PluginContext.AppConfigurations.Add(app => {
                    using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
                    {
                        using (var db = scope.ServiceProvider.GetRequiredService<TDbContext>())
                        {
                            startupAction.Invoke(db);
                        }
                    }
                });
            }
        }

        public void AddStartupAction(Action<IServiceProvider> action)
        {
            PluginContext.AppConfigurations.Add(app => {
                action.Invoke(app.ApplicationServices);
            });
        }

        public void AddStartupAction<TStartupAction>() where TStartupAction : class, IPluginStartupAction
        {
            PluginContext.ServiceConfigurations.Add(services => {
                services.AddTransient<IPluginStartupAction, TStartupAction>();
                services.AddTransient<TStartupAction>();
            });
        }

        public void ConfigureServices(Action<IServiceCollection> serviceConfig)
        {
            PluginContext.ServiceConfigurations.Add(serviceConfig);
        }

        public void ConfigureRoutes(Action<IRouteBuilder> routeConfig)
        {
            PluginContext.RouteConfigurations.Add(routeConfig);
        }
     
        public void ConfigureApplication(Action<IApplicationBuilder> appConfig)
        {
            PluginContext.AppConfigurations.Add(appConfig);
        }
    }
}
