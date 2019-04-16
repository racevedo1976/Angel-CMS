using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Common.Migrations;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Web.Migrations.IdentityDb;
using Angelo.Identity;
using Angelo.Identity.Models;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A24206_SeedCorpUserClaim : IAppMigration
    {
        private IdentityDbContext _identityDb;

        public string Id { get; } = "A24206";

        public string Migration { get; } = "Seed Corp User Claim";

        public A24206_SeedCorpUserClaim(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }
        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            await AddCorpUserClaimToCorpRoles();


            return MigrationResult.Success("Successfully added the corp User");
        }

        private async Task<bool> AddCorpUserClaimToCorpRoles()
        {
            var roles = new List<string>()
            {
                DbKeys.RoleNames.CorpAdmins,
                DbKeys.RoleNames.CorpSupport,
                DbKeys.RoleNames.CorpAccounting,
            };

            var corpRoles = _identityDb.Roles.Include(r => r.RoleClaims).Where(x => roles.Contains(x.Name)).ToList();

            if (corpRoles.Any())
            {
                foreach (var corpRole in corpRoles)
                {
                    if (corpRole.RoleClaims.All(x => x.ClaimType != CorpClaimTypes.CorpUser))
                    {
                        corpRole.RoleClaims.Add(new RoleClaim()
                            {ClaimType = CorpClaimTypes.CorpUser, ClaimValue = ConnectCoreConstants.CorporateId});
                    }
                }

                await _identityDb.SaveChangesAsync();
            }

            return await Task.FromResult(true);

        }
    }
}
