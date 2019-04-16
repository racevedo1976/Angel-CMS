using System;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Angelo.Common.Migrations;
using Angelo.Connect.Data;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A10040_EnsureConnectDb : IAppMigration
    {
        public string Id { get; } = "A10040";

        public string Migration { get; } = "Ensure Connect Db Exists";


        private ConnectDbContext _connectDb;

        public A10040_EnsureConnectDb(ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            if (_connectDb.Database.TryTestConnection() == true)
                return MigrationResult.Skipped();


            var cnInfo = _connectDb.Database.GetDbConnection();

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
