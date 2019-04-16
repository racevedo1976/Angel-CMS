using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Common.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Identity;
using Angelo.Identity.Models;


namespace Angelo.Connect.Web.Migrations.Application
{
    public class A26021_UpdateBoostMobileName : IAppMigration
    {
        
        public string Id { get; } = "A26021";

        public string Migration { get; } = "Fix Boost Mobile Name Typo";
        
        private IdentityDbContext _identityDb;
        
        public A26021_UpdateBoostMobileName(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if (_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Update the name for the wireless provider
            await _identityDb.Database.ExecuteNonQueryAsync($"UPDATE [auth].[WirelessProvider] SET [Name] = 'Boost Mobile' WHERE [Id] = '{DbKeys.WirelessProviderIds.BoostMobile}'");
            
            return MigrationResult.Success($"Updated Boost Mobile Name from  Boos Mobile to Boost Mobile");
        }
    }
}




