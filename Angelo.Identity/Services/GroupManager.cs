using Angelo.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Angelo.Identity.Services
{
    public class GroupManager
    {
        private IdentityDbContext _db;
        public GroupManager(IdentityDbContext db)
        {
            _db = db;
        }

        public async Task<Group> GetGroup(string groupId)
        {
            return await _db.Groups.FirstOrDefaultAsync(x => x.Id == groupId);
        }

        public async Task<IList<Group>> GetGroupsOwnedByUser(string userId)
        {
            return await _db.Groups.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<GroupMembership>> GetGroupMembers(string groupId)
        {
            var groupMemberships = await _db.GroupMemberships
                                            .Include(x => x.User)
                                            .Where(x => x.GroupId == groupId).ToListAsync();


            return groupMemberships;
        }

        public async Task<IEnumerable<GroupClaim>> GetGroupClaimsAsync(string groupId)
        {

            return await _db.GroupClaims.Where(x => x.GroupId == groupId).ToListAsync();
        }
        public IEnumerable<GroupClaim> GetGroupClaims(string groupId)
        {

            return _db.GroupClaims.Where(x => x.GroupId == groupId).ToList();
        }

        public async Task<IEnumerable<GroupClaim>> GetGroupClaimsAsync(string groupId, string claimType)
        {
            Ensure.Argument.NotNull(groupId);
            Ensure.Argument.NotNull(claimType);
            return await _db.GroupClaims.Where(x => x.GroupId == groupId && x.ClaimType == claimType).ToListAsync();
        }

        public async Task<IEnumerable<GroupClaim>> GetGroupClaimsAsync(Claim claim)
        {
            Ensure.Argument.NotNull(claim);

            return await _db.GroupClaims.Where(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value).ToListAsync();
        }


        public async Task<IdentityResult> AddClaimAsync(string groupId, Claim claim, string claimScope = null)
        {
            Ensure.Argument.NotNull(groupId);
            Ensure.Argument.NotNull(claim);

            IdentityResult result;
            try
            {
                _db.GroupClaims.Add(new GroupClaim()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    GroupId = groupId,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });

                await _db.SaveChangesAsync();

                return IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }


        public async Task UpdateGroupAsync(Group model)
        {
            Ensure.NotNull(model, "[GroupManager] Group Object is required.");

            var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (group != null)
            {
                group.Name = model.Name;
                _db.Groups.Update(group);
                await _db.SaveChangesAsync();
            }
        }

        public async Task AddUserToGroup(GroupMembership groupMembership)
        {
            if ((!string.IsNullOrEmpty(groupMembership.GroupId)) && (!string.IsNullOrEmpty(groupMembership.UserId)))
            {
                _db.GroupMemberships.Add(groupMembership);
                await _db.SaveChangesAsync();
            }
        }

        public async Task InserGroupAsync(Group model)
        {
            Ensure.NotNull(model, "[GroupManager] Group Object is required.");

            _db.Groups.Add(model);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Group>> GetUserMembershipsAsync(string userId)
        {
            var groupMemberships = await _db.GroupMemberships
                                        .Include(x => x.Group)
                                        .Where(x => x.UserId == userId).ToListAsync();

            var groups = groupMemberships.Select(x => x.Group);

            return groups;
        }
        public IEnumerable<Group> GetUserMemberships(string userId)
        {
            var groupMemberships = _db.GroupMemberships
                                        .Include(x => x.Group)
                                        .Where(x => x.UserId == userId).ToList();

            var groups = groupMemberships.Select(x => x.Group);

            return groups;
        }

        public async Task<GroupMembership> RemoveUserFromGroup(GroupMembership groupMembership)
        {
            var membership = _db.GroupMemberships.FirstOrDefault(x => x.UserId == groupMembership.UserId && x.GroupId == groupMembership.GroupId);

            if (membership != null)
            {
                _db.Remove(membership);
                await _db.SaveChangesAsync();
            }

            return groupMembership;
        }

        public async Task RemoveGroup(string id)
        {
            var memberships = _db.GroupMemberships.Where(x => x.GroupId == id).ToList();

            var group = await _db.Groups.FirstOrDefaultAsync(x => x.Id == id);

            _db.RemoveRange(memberships);
            _db.Remove(group);
            await _db.SaveChangesAsync();
        }

        public async Task<IdentityResult> RemoveClaimAsync(string claimId)
        {
            {
                Ensure.Argument.Is(claimId != "", "claimId is required");
                IdentityResult result;

                var claim = await _db.GroupClaims.FirstOrDefaultAsync(x => x.Id == claimId);

                if (claim == null)
                {
                    result = IdentityResult.Failed(new IdentityError()
                    {
                        Description = $"No claim with id {claimId} exists to delete."
                    });
                }
                else
                {
                    try
                    {
                        _db.GroupClaims.Remove(claim);
                        await _db.SaveChangesAsync();

                        result = IdentityResult.Success;
                    }
                    catch (Exception ex)
                    {
                        result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
                    }
                }
                return result;
            }
        }
    }
}