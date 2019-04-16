using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.NavMenu.Data
{
    public static class NavMenuDbActions
    {

        public static void CreateSchemas(NavMenuDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(NavMenuDbContext dbContext)
        {
            if (!dbContext.Database.TableExists("plugin.NavMenuWidget"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[NavMenuWidget](
	                    [Id] [nvarchar](50) NOT NULL,
	                    [Title] [nvarchar](500) NULL,
	                    [NavMenuId] [nvarchar](500) NULL,
	                    CONSTRAINT [PK_NavMenuWidget] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }
        }

        public static void InsertSeedData(NavMenuDbContext dbContext)
        {

            //dbContext.SaveChanges();
        }

    }
}
