using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Identity;

namespace Angelo.Connect.Calendar.Data
{
    public class P480005_UpdateEventAuthorClaimType : IAppMigration
    {
        private IdentityDbContext _identityDb;

        public string Id { get; } = "P480005";

        public string Migration { get; } = "Update Event Author ClaimType";

        public P480005_UpdateEventAuthorClaimType(IdentityDbContext identityDb)
        {
            _identityDb = identityDb;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            // Fail if cannot connect to db
            if (_identityDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            var oldClaimType = "resource-calendar-event-edit";
            var newClaimType = Calendar.Security.CalendarClaimTypes.CalendarAuthor;

            // Update any existing user claims
            await _identityDb.Database.ExecuteNonQueryAsync($"UPDATE [auth].[UserClaim] SET [ClaimType] = '{newClaimType}' WHERE [ClaimType] = '{oldClaimType}'");

            // Update any existing role claims
            await _identityDb.Database.ExecuteNonQueryAsync($"UPDATE [auth].[RoleClaim] SET [ClaimType] = '{newClaimType}' WHERE [ClaimType] = '{oldClaimType}'");

            // Update any existing group claims
            await _identityDb.Database.ExecuteNonQueryAsync($"UPDATE [auth].[GroupClaim] SET [ClaimType] = '{newClaimType}' WHERE [ClaimType] = '{oldClaimType}'");


            return MigrationResult.Success($"Updated ClaimType {oldClaimType} to {newClaimType}");
        }
    }
}
