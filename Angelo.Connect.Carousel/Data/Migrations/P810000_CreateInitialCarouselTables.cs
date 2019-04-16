using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Angelo.Connect.Carousel.Data;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Carousel.Data.Migrations
{
    public class P810000_CreateInitialDocumentTables : IAppMigration
    {
        public string Id { get; } = "P810000";

        public string Migration { get; } = "Create Initial Carousel Tables";

        private CarouselDbContext _dbContext;

        public P810000_CreateInitialDocumentTables(CarouselDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_dbContext.Database.TableExists("CarouselWidget", "plugin"))
                return MigrationResult.Skipped();


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[CarouselWidget](
	                    [Id] [nvarchar](50) NOT NULL,
                        [SiteId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NULL,
                        CONSTRAINT [PK_CarouselWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");

            if (_dbContext.Database.TableExists("CarouselSlide", "plugin"))
                return MigrationResult.Skipped();

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                    CREATE TABLE [plugin].[CarouselSlide](
                        [Id] [nvarchar](50) NOT NULL,
                        [WidgetId] [nvarchar](50) NOT NULL,
                        [Title] [nvarchar](500) NOT NULL,
                        [Description] [nvarchar](MAX) NULL,
                        [LinkText] [nvarchar](2048) NULL,
                        [LinkUrl] [nvarchar](2048) NULL,
                        [LinkTarget] [nvarchar] (7) NOT NULL DEFAULT '_self',
                        [Sort] [int] NOT NULL DEFAULT(0),
                        CONSTRAINT [PK_CarouselSlide] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");


            return MigrationResult.Success();
        }
    }
}
