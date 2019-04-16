using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Video.Data
{
    public class P710000_CreateInitialVideoTables : IAppMigration
    {
        private VideoDbContext _dbContext;

        public string Id { get; } = "P710000";

        public string Migration { get; } = "Create Initial Video Tables";

        public P710000_CreateInitialVideoTables(VideoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            if (_dbContext.Database.TableExists("VideoWidget", "plugin"))
                return MigrationResult.Skipped();

            // else create the tables
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[VideoWidget](
	                [Id] [nvarchar](50) NOT NULL,
	                [Title] [nvarchar](500) NULL,
                    [VideoSourceType] [nvarchar](500) NULL,
	                [VideoId] [nvarchar](500) NULL,
                    [VideoUrl] [nvarchar](500) NULL,
	                CONSTRAINT [PK_VideoLink] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[VideoStreamLink](
	                [Id] [nvarchar](50) NOT NULL,
                    [ClientId] [nvarchar](50) NULL,
	                [Title] [nvarchar](500) NULL,
	                [Path] [nvarchar](500) NULL,
	                CONSTRAINT [PK_VideoLinkSource] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            return MigrationResult.Success();
        }
    }
}
