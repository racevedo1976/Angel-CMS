using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Identity;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A10020_EnsureIdentityDb : IAppMigration
    {
        public string Id { get; } = "A10020";

        public string Migration { get; } = "Ensure Identity Db Exists";   

        private IdentityDbContext _identityDb;

        public A10020_EnsureIdentityDb(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_identityDb.Database.TryTestConnection() == true)
                return MigrationResult.Skipped();


            var cnInfo = _identityDb.Database.GetDbConnection();

            if (!string.IsNullOrEmpty(cnInfo?.ConnectionString))
            {
                var cnBuilder = new SqlConnectionStringBuilder(cnInfo.ConnectionString);
                var databaseName = cnBuilder.InitialCatalog;

                // can't connect to the db specified because it doesn't exist so setting
                // thus, connecting to master instead so we can create
                cnBuilder.InitialCatalog = "master";

                var connection = new SqlConnection(cnBuilder.ToString());

                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = $"CREATE DATABASE [{databaseName}]";
                    await command.ExecuteNonQueryAsync();
                }
                connection.Close();

                return MigrationResult.Success($"Created [{databaseName}]");
            }

            //else failed
            return MigrationResult.Failed("Connetion string missing");
        }
    }
}
