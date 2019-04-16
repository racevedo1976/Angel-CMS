using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Common.Migrations
{
    public static partial class StartupExtensions
    {

        public static IServiceCollection AddAppMigrationRepo(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<MigrationRepository>(options => {
                options.UseSqlServer(connectionString);
            });

            return services;
        }

        public static IServiceCollection AddAppMigration<TDbMigrationAction>(this IServiceCollection services)
        where TDbMigrationAction : class, IAppMigration
        {
            services.AddTransient<IAppMigration, TDbMigrationAction>();
            return services;
        }

    }
}
