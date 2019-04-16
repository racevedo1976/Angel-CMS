using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;

using Angelo.Common.Migrations;

namespace Angelo.Common.Migrations
{
    public static partial class StartupExtensions
    {
        public static IApplicationBuilder RunAppMigrations(this IApplicationBuilder app)
        {
            var registeredMigrations = app.ApplicationServices.GetServices<IAppMigration>()?
                .OrderBy(x => x.Id)
                .ThenBy(x => x.Migration);

            using (var scope = app.ApplicationServices.GetService<IServiceScopeFactory>().CreateScope())
            {              
                using (var migrationRepo = scope.ServiceProvider.GetRequiredService<MigrationRepository>())
                {
                    if (migrationRepo == null)
                        throw new NullReferenceException(nameof(MigrationRepository));

                    foreach (var migration in registeredMigrations)
                    {
                        // Skip migrations that have already been applied
                        if (migrationRepo.HasHistory(migration).Result)
                            continue;

                        // else, Run the migration and log results
                        MigrationResult result = null;
                        DateTime started = DateTime.Now;

                        try
                        {
                            result = migration.ExecuteAsync().Result;
                        }
                        catch(Exception ex)
                        {
                            result = MigrationResult.Failed(ex.Message);
                        }
                           
                        migrationRepo.LogHistory(migration, result, started, finished: DateTime.Now);
                    }

                    migrationRepo.SaveChanges();
                }
            }
                 
            return app;
        }

    }
}
