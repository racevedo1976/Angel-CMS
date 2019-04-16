using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Angelo.Common.Extensions
{
    public static class DbContextExtensions
    {
        public static DbContext EnsureMigrated(this DbContext db)
        {
            db.Database.Migrate();
            return db;
        }

        public static bool RequiresMigration(this DbContext context)
        {
            var applied = context.GetService<IHistoryRepository>()
                .GetAppliedMigrations()
                .Select(m => m.MigrationId);

            var total = context.GetService<IMigrationsAssembly>()
                .Migrations
                .Select(m => m.Key);

            return total.Except(applied).Any();
        }
    }
}
