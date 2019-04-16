using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Angelo.Common.Abstractions;

namespace Angelo.Connect.Extensions
{
    public static class CommonStartupExtensions
    {       
        public static IServiceCollection AddStartupAction<TStartupAction>(this IServiceCollection services) 
        where TStartupAction : class, IStartupAction
        {
            services.AddTransient<IStartupAction, TStartupAction>();
            services.AddTransient<TStartupAction>();

            return services;
        }

        public static IApplicationBuilder RunStartupActions(this IApplicationBuilder app)
        {
            var startupClasses = app.ApplicationServices.GetServices<IStartupAction>();

            startupClasses.ToList().ForEach(x => x.Invoke());

            return app;
        }

    }
}
