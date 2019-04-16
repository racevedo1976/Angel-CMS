using System;
using Microsoft.Extensions.Options;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Menus;
using Angelo.Connect.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MenuStartupExtensions
    {
        public static IServiceCollection AddConnectCoreMenus(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IMenuItemProvider, CorpMenu>();
            services.AddScoped<IMenuItemProvider, ClientMenu>();
            services.AddScoped<IMenuItemProvider, SiteMenu>();
            services.AddScoped<IMenuItemProvider, LibraryMenu>();
            services.AddScoped<IMenuItemProvider, UserOptionsMenu>();
            services.AddScoped<IMenuItemProvider, UserContentMenu>();

            services.AddTransient<MenuProvider>();
         
            return services;
        }

     
    }
}
