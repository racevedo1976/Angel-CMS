using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Announcement.Data
{
    public class P310000_CreateInitialAnnouncementTables : IAppMigration
    {
        private AnnouncementDbContext _dbContext;

        public string Id { get; } = "P310000";

        public string Migration { get; } = "Create Initial Announcement Tables";

        public P310000_CreateInitialAnnouncementTables(AnnouncementDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            if (_dbContext.Database.TableExists("AnnouncementPost", "plugin"))
                return MigrationResult.Skipped();

            // else create the tables
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementPost](
	                [Id] [nvarchar](450) NOT NULL,
                    [DocumentId] [nvarchar](50) NOT NULL,
                    [ContentTreeId] [nvarchar](50) NULL,
	                [Title] [nvarchar](500) NOT NULL,
                    [Image] [nvarchar](255) NULL,
                    [Caption] [nvarchar](1000) NULL,
                    [Excerp] [nvarchar](MAX) NULL,
                    [Content] [nvarchar](MAX) NULL,
                    [Posted] [datetime] NOT NULL,
                    [UserId] [nvarchar](450) NOT NULL,
                    [PrivateCommentsAllowed] [bit] NOT NULL,
                    [PrivateCommentsModerated] [bit] NOT NULL,
                    [PublicCommentsAllowed] [bit] NOT NULL,
                    [PublicCommentsModerated] [bit] NOT NULL,
	                CONSTRAINT [PK_AnnouncementPost] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementPostTag](
                    [Id] [nvarchar](450) NOT NULL,
	                [PostId] [nvarchar](450) NOT NULL,
                    [TagId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_AnnouncementPostTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementPostCategory](
                    [Id] [nvarchar](450) NOT NULL,
	                [PostId] [nvarchar](450) NOT NULL,
                    [CategoryId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_AnnouncementPostCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementCategory](
                    [Id] [nvarchar](450) NOT NULL,
                    [Title] [nvarchar](450) NOT NULL,
                    [UserId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_AnnouncementCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementWidget](
	                [Id] [nvarchar](450) NOT NULL,
                    [Title] [nvarchar](100) NOT NULL,
                    [PageSize] [int] NOT NULL,
                    [CreateAnnouncement] [bit] NOT NULL,
                    [AnnouncementId] [nvarchar] (450),
                                             
	                CONSTRAINT [PK_AnnouncementWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementWidgetTag](
                    [Id] [nvarchar](450) NOT NULL,
	                [WidgetId] [nvarchar](450) NOT NULL,
                    [TagId] [nvarchar](450) NOT NULL,
	                CONSTRAINT [PK_AnnouncementWidgetTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[AnnouncementWidgetCategory](
                    [Id] [nvarchar](450) NOT NULL,
	                [WidgetId] [nvarchar](450) NOT NULL,
                    [CategoryId] [nvarchar](450) NOT NULL,
	                CONSTRAINT [PK_AnnouncementWidgetCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            return MigrationResult.Success();
        }
    }
}
