using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Abstractions;
using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Services
{
    public class FolderSecurityManager : IFolderSecurityManager
    {
        private ConnectDbContext _db;

        public FolderSecurityManager(ConnectDbContext db)
        {
            Ensure.NotNull(db, $"{nameof(db)} cannot be null.");
            _db = db;
        }

        //public async Task<IEnumerable<IContentClaim>> GetFolderClaimsAsync(IFolder folder)
        //{
        //    return await _db.ResourceClaims.Where(x => x.ResourceId == folder.Id).ToListAsync();
        //}

        //public async Task AddFolderClaimAsync(IFolder folder, IContentClaim claim)
        //{
        //    var exists = await HasClaimAsync(folder, claim);

        //    // TODO: Consider throwing an error vs. not adding
        //    if (!exists)
        //    {
        //        var folderClaim = BuildFolderClaim(folder, claim);

        //        _db.ResourceClaims.Add(folderClaim);
        //        await _db.SaveChangesAsync();
        //    }
        //}

        //public async Task RemoveFolderClaimAsync(IFolder folder, IContentClaim claim)
        //{
        //    var entity = await _db.ResourceClaims.FirstOrDefaultAsync(x =>
        //        x.FolderId == folder.Id
        //        && x.OwnerLevel == claim.OwnerLevel
        //        && x.OwnerId == claim.OwnerId
        //        && x.ClaimType == claim.ClaimType
        //    );

        //    // TODO: Consider throwing an error vs. not adding
        //    if (entity != null)
        //    {
        //        _db.ResourceClaims.Remove(entity);
        //        await _db.SaveChangesAsync();
        //    }
        //}

        //public async Task SetFolderClaimsAsync(IFolder folder, IEnumerable<IContentClaim> claims)
        //{
        //    _db.ResourceClaims.RemoveRange(
        //        await _db.ResourceClaims.Where(x => x.FolderId == folder.Id).ToListAsync()
        //    );

        //    _db.ResourceClaims.AddRange(
        //        claims.Select(c => BuildFolderClaim(folder, c))
        //    );

        //    await _db.SaveChangesAsync();
        //}

        //private async Task<bool> HasClaimAsync(IFolder folder, IContentClaim claim)
        //{
        //    return await _db.ResourceClaims.AnyAsync(x =>
        //        x.FolderId == folder.Id
        //        && x.OwnerLevel == claim.OwnerLevel
        //        && x.OwnerId == claim.OwnerId
        //        && x.ClaimType == claim.ClaimType
        //    );
        //}

        //private FolderClaim BuildFolderClaim(IFolder folder, IContentClaim claim)
        //{
        //    return new FolderClaim
        //    {
        //        Id = KeyGen.NewGuid(),
        //        FolderId = folder.Id,
        //        OwnerLevel = claim.OwnerLevel,
        //        OwnerId = claim.OwnerId,
        //        ClaimType = claim.ClaimType,
        //    };
        //}
    }
}
