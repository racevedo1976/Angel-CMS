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
    public class A26020_UpdateUserLibraryClaims : IAppMigration
    {
        // NOTE: This is the data update portion of A10201

        public string Id { get; } = "A26020";

        public string Migration { get; } = "Add user library claims";
     
        private IdentityDbContext _identityDb;
        private ConnectDbContext _connectDb;

        public A26020_UpdateUserLibraryClaims(IdentityDbContext identityDb, ConnectDbContext connectDb)
        {
            _identityDb = identityDb;
            _connectDb = connectDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if (_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // Insert claim into Client RoleClaims
            var clients = await _connectDb.Clients.ToListAsync();

            foreach (Client client in clients)
            {
                var roles = await _identityDb.Roles.Where(x => x.PoolId == client.SecurityPoolId && x.Name != "Registered Users").ToListAsync();
                var clientId = client.Id;

                foreach (Role role in roles)
                {
                    await _identityDb.Database.ExecuteSqlCommandAsync($@"
                        INSERT INTO auth.RoleClaim (RoleId, ClaimType, ClaimValue)
                        VALUES ('{role.Id}', 'user-library-owner', '{clientId}')
                    ");
                }
            }

            // Insert claim into Site RoleClaims
            var sites = await _connectDb.Sites.ToListAsync();

            foreach (Site site in sites)
            {
                var roles = await _identityDb.Roles.Where(x => x.PoolId == site.SecurityPoolId && x.Name != "Registered Users").ToListAsync();
                var siteId = site.Id;

                foreach (Role role in roles)
                {
                    await _identityDb.Database.ExecuteSqlCommandAsync($@"
                        INSERT INTO auth.RoleClaim (RoleId, ClaimType, ClaimValue)
                        VALUES ('{role.Id}', 'user-library-owner', '{siteId}')
                    ");
                }
            }


            return MigrationResult.Success("Added user-library-owner claim to roles.");
        }
    }
}
