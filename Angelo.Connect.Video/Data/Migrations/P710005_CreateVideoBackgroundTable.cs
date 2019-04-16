using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Video.Data
{
    public class P710005_CreateVideoBackgroundTable : IAppMigration
    {
        private VideoDbContext _dbContext;

        public string Id { get; } = "P710005";

        public string Migration { get; } = "Create Video Background table";

        public P710005_CreateVideoBackgroundTable(VideoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            if (_dbContext.Database.TableExists("VideoBackgroundWidget", "plugin"))
                return MigrationResult.Skipped();

            // else create the tables
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[VideoBackgroundWidget](
	                [Id] [nvarchar](50) NOT NULL,
                    [VideoSourceType] [nvarchar](500) NULL,
                    [YoutubeVideoId] [nvarchar](50) NULL,
                    [VimeoVideoId] [nvarchar](50) NULL,
                    [Positioning] [nvarchar](500) NULL,
                    [Autoplay] [bit] DEFAULT (1) NOT NULL,
                    [ShowPlayerControls] [bit] DEFAULT (0) NOT NULL,
                    [VideoUrl] [nvarchar](500) NULL,
	                CONSTRAINT [PK_VideoBackgroundLink] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            return MigrationResult.Success();
        }
    }
}
