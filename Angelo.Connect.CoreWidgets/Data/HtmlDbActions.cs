using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.CoreWidgets.Data
{
    public static class HtmlDbActions
    {
        public static void CreateSchemas(HtmlDbContext dbContext)
        {
            dbContext.Database.ExecuteSqlCommand(@"
                IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'plugin')
                BEGIN
                    EXEC('CREATE SCHEMA plugin')
                END
            ");
        }

        public static void CreateTables(HtmlDbContext dbContext)
        {

            if (!dbContext.Database.TableExists("plugin.NavBar"))
            {
                dbContext.Database.ExecuteSqlCommand(@"
                    CREATE TABLE [plugin].[NavBar] (
                        [Id]   NVARCHAR (50)  NOT NULL,
                        [ItemWidth] NVARCHAR (50) NULL,
                        [NavMenuId] NVARCHAR (50) NULL,
                        CONSTRAINT [PK_NavBar] PRIMARY KEY CLUSTERED ([Id] ASC)
                    )
                ");
            }
        }
    }
}
