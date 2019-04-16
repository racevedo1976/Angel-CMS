using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using System.Threading.Tasks;

namespace Angelo.Connect.Blog.Data
{
    public class P410000_CreateInitialBlogTables : IAppMigration
    {
        private BlogDbContext _dbContext;

        public string Id { get; } = "P410000";

        public string Migration { get; } = "Create Initial Blog Tables";

        public P410000_CreateInitialBlogTables(BlogDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {

            if (_dbContext.Database.TableExists("BlogPost", "plugin"))
                return MigrationResult.Skipped();

            // else create the tables
            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogPost](
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
	                CONSTRAINT [PK_BlogPost] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogPostTag](
                    [Id] [nvarchar](450) NOT NULL,
	                [PostId] [nvarchar](450) NOT NULL,
                    [TagId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_BlogPostTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogPostCategory](
                    [Id] [nvarchar](450) NOT NULL,
	                [PostId] [nvarchar](450) NOT NULL,
                    [CategoryId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_BlogPostCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogCategory](
                    [Id] [nvarchar](450) NOT NULL,
                    [Title] [nvarchar](450) NOT NULL,
                    [UserId] [nvarchar](450) NOT NULL,
                    [IsActive] [bit] NOT NULL,
	                CONSTRAINT [PK_BlogCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogWidget](
	                [Id] [nvarchar](450) NOT NULL,
                    [Title] [nvarchar](100) NOT NULL,
                    [PageSize] [int] NOT NULL,
                    [CreateBlog] [bit] NOT NULL,
                    [BlogId] [nvarchar] (450),
                                             
	                CONSTRAINT [PK_BlogWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogWidgetTag](
                    [Id] [nvarchar](450) NOT NULL,
	                [WidgetId] [nvarchar](450) NOT NULL,
                    [TagId] [nvarchar](450) NOT NULL,
	                CONSTRAINT [PK_BlogWidgetTag] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");

            await _dbContext.Database.ExecuteSqlCommandAsync(@"
                CREATE TABLE [plugin].[BlogWidgetCategory](
                    [Id] [nvarchar](450) NOT NULL,
	                [WidgetId] [nvarchar](450) NOT NULL,
                    [CategoryId] [nvarchar](450) NOT NULL,
	                CONSTRAINT [PK_BlogWidgetCategory] PRIMARY KEY CLUSTERED ([Id] ASC)
                )
            ");


            return MigrationResult.Success();
        }
    }
}
