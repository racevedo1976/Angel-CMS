using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.News.Data
{
    public class P910000_CreateInitialNewsTables : IAppMigration
    {
        private NewsDbContext _dbContext;

        public string Id { get; } = "P910000";

        public string Migration { get; } = "Create Initial News Tables";

        public P910000_CreateInitialNewsTables(NewsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            if (_dbContext.Database.TableExists("NewsPost", "plugin"))
                return MigrationResult.Skipped();

            // else create the tables
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsPost](
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
	                CONSTRAINT [PK_NewsPost] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsPostTag](
                    [Id] [nvarchar](450) NOT NULL,
	                [PostId] [nvarchar](450) NOT NULL,
                    [TagId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_NewsPostTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsPostCategory](
                    [Id] [nvarchar](450) NOT NULL,
	                [PostId] [nvarchar](450) NOT NULL,
                    [CategoryId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_NewsPostCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsCategory](
                    [Id] [nvarchar](450) NOT NULL,
                    [Title] [nvarchar](450) NOT NULL,
                    [UserId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_NewsCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsWidget](
	                [Id] [nvarchar](450) NOT NULL,
                    [Title] [nvarchar](100) NOT NULL,
                    [PageSize] [int] NOT NULL,
                    [CreateNews] [bit] NOT NULL,
                    [NewsId] [nvarchar] (450),
                                             
	                CONSTRAINT [PK_NewsWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsWidgetTag](
                    [Id] [nvarchar](450) NOT NULL,
	                [WidgetId] [nvarchar](450) NOT NULL,
                    [TagId] [nvarchar](450) NOT NULL,
	                CONSTRAINT [PK_NewsWidgetTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[NewsWidgetCategory](
                    [Id] [nvarchar](450) NOT NULL,
	                [WidgetId] [nvarchar](450) NOT NULL,
                    [CategoryId] [nvarchar](450) NOT NULL,
	                CONSTRAINT [PK_NewsWidgetCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            return MigrationResult.Success();
        }
    }
}
