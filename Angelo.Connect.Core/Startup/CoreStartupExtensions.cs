using System;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Logging;
using Angelo.Connect.Services;
using Angelo.Connect.Configuration;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Widgets;
using Angelo.Connect.Widgets.Services;
using Angelo.Connect.UI;
using Angelo.Connect.UserConsole;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CoreStartupExtensions
    {
        public static IServiceCollection AddConnectCore(this IServiceCollection services)
        {
            return AddConnectCore(services, null);
        }

        public static IServiceCollection AddConnectCore(this IServiceCollection services, Action<ConnectCoreOptions> optionsDelegate)
        {
            if (services == null)
                throw new ArgumentNullException("ServiceCollection");

            var connectCoreOptions = new ConnectCoreOptions();

            if(optionsDelegate != null)
                optionsDelegate.Invoke(connectCoreOptions);

            // registering options as singleton
            services.AddSingleton(connectCoreOptions);

            // registering context caches as singletons
            //services.AddSingleton<ContextCache<ProductContext>>();

            // register connect core services here
            services.AddTransient<ClientManager>();
            services.AddTransient<ProductManager>();
            services.AddTransient<SiteManager>();
            services.AddTransient<SitePublisher>();
            services.AddTransient<SiteCollectionManager>();
            services.AddTransient<PageManager>();
            services.AddTransient<PageMasterManager>();
            services.AddTransient<UserProfileManager>();
            services.AddTransient<NavigationMenuManager>();
            services.AddTransient<NotificationManager>();
            services.AddTransient<NotificationProcessor>();
            services.AddTransient<UserGroupManager>();
            services.AddTransient<EnumLocalizer>();

            // Site Templates
            //Templates
            services.AddSingleton<SiteTemplateManager>();
            services.AddTransient<SiteTemplateExporter>();

            // Content Services
            services.AddTransient<ContentManager>();
            services.AddTransient<WidgetProvider>();
            services.AddTransient<IWidgetNamedModelProvider, JsonNamedModelProvider>();

            // UserConsole Providers
            services.AddTransient<IUserConsoleComponentProvider, UserConsoleCustomProvider>();
            services.AddTransient<IUserConsoleComponentProvider, UserConsoleTreeProvider>();
            services.AddTransient<UserConsoleComponentFactory>();

            // NavMenu Content Providers
            services.AddTransient<INavMenuContentProvider, NavMenuPageProvider>();
            //services.AddTransient<INavMenuContentProvider, NavMenuTestProvider>();

            // Route helper
            services.AddScoped<Routes>();

            // DB Logger
            services.AddTransient<DbLoggerProvider>();
            services.AddTransient<DbLogService>();
            services.AddTransient<DbLogContext>();
            services.AddTransient<DbLogger>();
                     
            return services;
        }

        public static IServiceCollection AddAdminContext<TAdminContextAccessor>(this IServiceCollection services)
        where TAdminContextAccessor : class, IContextAccessor<AdminContext>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // this can be scoped or transient since it's just bundles the other accessors
            services.AddTransient<IContextAccessor<AdminContext>, TAdminContextAccessor>();
            services.AddTransient<TAdminContextAccessor>();

            //TODO: Remove - should take a dependency on the accessor only
            services.AddTransient<AdminContext>(serviceProvider => {
                var accessor = serviceProvider.GetService<TAdminContextAccessor>();

                return accessor.GetContext();
            });

            return services;
        }

        public static IServiceCollection AddClientAdminContext<TClientAdminContextAccessor>(this IServiceCollection services)
        where TClientAdminContextAccessor : class, IContextAccessor<ClientAdminContext>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // These must be scoped in order to persist context throughout duration of request
            services.AddScoped<IContextAccessor<ClientAdminContext>, TClientAdminContextAccessor>();
            services.AddScoped<TClientAdminContextAccessor>();

            //TODO: Remove - should take a dependency on the accessor only
            services.AddTransient<ClientAdminContext>(serviceProvider => {
                var accessor = serviceProvider.GetService<TClientAdminContextAccessor>();

                return accessor.GetContext();
            });

            return services;
        }

        public static IServiceCollection AddSiteAdminContext<TSiteAdminContextAccessor>(this IServiceCollection services)
        where TSiteAdminContextAccessor : class, IContextAccessor<SiteAdminContext>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // These must be scoped in order to persist context throughout duration of request
            services.AddScoped<IContextAccessor<SiteAdminContext>, TSiteAdminContextAccessor>();
            services.AddScoped<TSiteAdminContextAccessor>();

            //TODO: Remove - should take a dependency on the accessor only
            services.AddTransient<SiteAdminContext>(serviceProvider => {
                var accessor = serviceProvider.GetService<TSiteAdminContextAccessor>();

                return accessor.GetContext();
            });


            return services;
        }

        public static IServiceCollection AddSiteContext<TSiteContextAccessor>(this IServiceCollection services)
        where TSiteContextAccessor : class, IContextAccessor<SiteContext>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            // These must be scoped in order to persist context throughout duration of request
            services.AddScoped<IContextAccessor<SiteContext>, TSiteContextAccessor>();
            services.AddScoped<TSiteContextAccessor>();

            return services;
        }

    }
}
