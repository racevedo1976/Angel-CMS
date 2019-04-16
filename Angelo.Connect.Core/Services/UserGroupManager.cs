using Angelo.Common.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Angelo.Connect.Services
{
    public class UserGroupManager
    {
        private ConnectDbContext _connectDb;
        private IdentityDbContext _identityDb;
        private UserContext _userContext;
        private SiteManager _siteManager;

        public UserGroupManager(ConnectDbContext connectDb, IdentityDbContext identityDb, UserContext userContext,
            SiteManager siteManager)
        {
            _connectDb = connectDb;
            _identityDb = identityDb;
            _userContext = userContext;
            _siteManager = siteManager;
        }

        public IQueryable<UserGroup> GetUserGroupsOfOwnerQuery(OwnerLevel ownerLevel, string ownerId)
        {
            var query = _connectDb.UserGroups.Where(x => (x.OwnerLevel == ownerLevel) && (x.OwnerId == ownerId));
            return query;
        }

        public IQueryable<UserGroup> GetUserGroupsOfOwnerAndTypeQuery(OwnerLevel ownerLevel, string ownerId, UserGroupType userGroupType)
        {
            var query = _connectDb.UserGroups.Where(x => (x.OwnerLevel == ownerLevel) && (x.OwnerId == ownerId) && (x.UserGroupType == userGroupType));
            return query;
        }

        public IQueryable<UserGroup> GetUserGroupsOfOwnerAndTypeQuery(string ownerId, UserGroupType userGroupType)
        {
            var query = _connectDb.UserGroups.Where(x => (x.OwnerId == ownerId) && (x.UserGroupType == userGroupType));
            return query;
        }

        public IQueryable<UserGroup> GetUserGroupsAssignedToUserWithAccessLevelQuery(string userId, AccessLevel[] accessLevels, UserGroupType userGroupType)
        {
            var memberships = _connectDb.UserGroupMemberships.AsNoTracking()
                .Where(x => x.UserId == userId)
                .Where(x => accessLevels.Contains(x.AccessLevel));
            
            var assignedGroups = _connectDb.UserGroups.AsNoTracking()
                .Where(x => x.UserGroupType == userGroupType)
                .Join(memberships,
                    g => g.Id,
                    m => m.UserGroupId,
                    (ug, gm) => ug
                    );

            return assignedGroups;
        }

        public IQueryable<UserGroupMembership> GetUserGroupMembershipQuery(string userGroupId)
        {
            var query = _connectDb.UserGroupMemberships.Where(x => (x.UserGroupId == userGroupId));
            return query;
        }

        public async Task<UserGroup> GetUserGroupAsync(string id)
        {
            var group = await _connectDb.UserGroups.Where(x => x.Id == id).FirstOrDefaultAsync();
            return group;
        }

        public async Task<UserGroupMembership> GetUserGroupMembershipAsync(string userGroupId, string userId)
        {
            var membership = await _connectDb.UserGroupMemberships.Where(x => (x.UserGroupId == userGroupId) && (x.UserId == userId)).FirstOrDefaultAsync();
            return membership;
        }

        //public async Task<List<UserGroupMembership>> GetUserGroupMembershipsAsync(string userGroupId)
        //{
        //    var query = _connectDb.UserGroupMemberships.AsNoTracking();
        //        //.Join(_db.NotificationUserGroups.Where(x => x.NotificationId == notificationId),
        //        //    ug => ug.Id,
        //        //    ng => ng.UserGroupId,
        //        //    (ug, ng) => ug);

        //    var results = await query.ToListAsync();

        //    return results;
        //}

        public async Task UpdateUserGroupAsync(UserGroup userGroup)
        {
            var oldGroup = await _connectDb.UserGroups.Where(x => x.Id == userGroup.Id).FirstOrDefaultAsync();
            if (oldGroup == null)
                throw new Exception("Unable to find UserGroup (Id = " + userGroup.Id + ")");
            oldGroup.Name = userGroup.Name;
            oldGroup.AllowPublicEnrollment = userGroup.AllowPublicEnrollment;
            await _connectDb.SaveChangesAsync();
        }

        public async Task InsertUserGroupAsync(UserGroup userGroup)
        {
            userGroup.CreatedDT = DateTime.Now;
            userGroup.CreatedBy = _userContext.UserId;
            _connectDb.UserGroups.Add(userGroup);
            await _connectDb.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserGroupAsync(string userGroupId)
        {
            var members = await _connectDb.UserGroupMemberships
                .Where(m => m.UserGroupId == userGroupId)
                .ToListAsync();

            var group = await _connectDb.UserGroups
                .Where(g => g.Id == userGroupId)
                .FirstOrDefaultAsync();

            if (group == null)
                return false;

            _connectDb.UserGroups.Remove(group);
            _connectDb.UserGroupMemberships.RemoveRange(members);
            await _connectDb.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserGroup>> GetPublicUserGroupsForSiteAsync(string siteId, UserGroupType userGroupType)
        {
            var site = await _siteManager.GetByIdAsync(siteId);

            var siteGroups = await _connectDb.UserGroups.AsNoTracking()
                    .Where(g => (g.OwnerId == siteId) &&
                                (g.OwnerLevel == OwnerLevel.Site) &&
                                (g.AllowPublicEnrollment == true) &&
                                (g.UserGroupType == userGroupType))
                    .ToListAsync();

            var clientGroups = await _connectDb.UserGroups.AsNoTracking()
                    .Where(g => (g.OwnerId == site.ClientId) &&
                                (g.OwnerLevel == OwnerLevel.Client) &&
                                (g.AllowPublicEnrollment == true) &&
                                (g.UserGroupType == userGroupType))
                    .ToListAsync();

            var publicGroups = new List<UserGroup>();
            publicGroups.AddRange(siteGroups);
            publicGroups.AddRange(clientGroups);

            return publicGroups.OrderBy(g => g.Name).ToList();
        }

        public async Task<List<UserGroupMembership>> GetPublicUserGroupsForUserAndSiteAsync(string userId, string siteId, UserGroupType userGroupType)
        {
            var memberships = await _connectDb.UserGroupMemberships.AsNoTracking()
                                .Where(m => m.UserId == userId)
                                .Join(_connectDb.UserGroups.AsNoTracking()
                                        .Where(g => (g.OwnerId == siteId) &&
                                                    (g.OwnerLevel == OwnerLevel.Site) &&
                                                    (g.UserGroupType == userGroupType)),
                                    m => m.UserGroupId,
                                    g => g.Id,
                                    (m, g) => new UserGroupMembership()
                                    {
                                        UserGroupId = m.UserGroupId,
                                        UserId = m.UserId,
                                        AllowEmailMessaging = m.AllowEmailMessaging,
                                        AllowSmsMessaging = m.AllowSmsMessaging,
                                        UserGroup = new UserGroup()
                                        {
                                            Id = g.Id,
                                            Name = g.Name,
                                            AllowPublicEnrollment = g.AllowPublicEnrollment,
                                            UserGroupType = g.UserGroupType
                                        }
                                    }).ToListAsync();

            var allGroups = await _connectDb.UserGroups.AsNoTracking()
                    .Where(g => (g.OwnerId == siteId) &&
                                (g.OwnerLevel == OwnerLevel.Site) &&
                                (g.AllowPublicEnrollment == true) &&
                                (g.UserGroupType == userGroupType))
                    .OrderBy(g => g.Name)
                    .ToListAsync();

            // Add public groups that the user is not currently assigned to.
            foreach (var group in allGroups)
            {
                var curMembership = memberships.Where(m => m.UserGroupId == group.Id).FirstOrDefault();
                if (curMembership == null)
                {
                    memberships.Add(new UserGroupMembership()
                    {
                        UserId = null,  // null will indicate that this user does not belong to this group.
                        UserGroupId = group.Id,
                        AllowEmailMessaging = false,
                        AllowSmsMessaging = false,
                        UserGroup = group
                    });
                }
            }

            return memberships;
        }

        // Note: Set allowEmail and/or allowSms to null to keep the values currently stored in the database.
        public async Task<DbActionResult> UpdateNotificationGroupSubscriptionAsync(string userGroupId, string userId, bool? allowEmail = null, bool? allowSms = null)
        {
            var result = DbActionResult.Error;
            var userGroup = await GetUserGroupAsync(userGroupId);
            if (userGroup == null)
                throw new Exception($"UserGroup not found (UserGroupId:{userGroupId ?? ""}).");
            if (userGroup.AllowPublicEnrollment == false)
                throw new Exception($"UserGroup does not allow for public entrollment (UserGroupId:{userGroup.Id}, Name:{userGroup.Name}).");

            var oldMembership = await _connectDb.UserGroupMemberships
                .Where(x => (x.UserGroupId == userGroupId) && (x.UserId == userId))
                .FirstOrDefaultAsync();
            if (oldMembership == null)
            {
                var membership = new UserGroupMembership()
                {
                    UserGroupId = userGroupId,
                    UserId = userId,
                    AccessLevel = Identity.Models.AccessLevel.Audience,
                    AddedDT = DateTime.Now,
                    AddedBy = _userContext.UserId,
                    AllowEmailMessaging = allowEmail ?? false,
                    AllowSmsMessaging = allowSms ?? false
                };
                _connectDb.UserGroupMemberships.Add(membership);
                result = DbActionResult.Insert;
            }
            else
            {
                oldMembership.AllowEmailMessaging = allowEmail ?? oldMembership.AllowEmailMessaging;
                oldMembership.AllowSmsMessaging = allowSms ?? oldMembership.AllowSmsMessaging;
                result = DbActionResult.Update;
            }
            await _connectDb.SaveChangesAsync();
            return result;
        }

        public async Task<DbActionResult> SubscribeToNotificationGroupAsync(string userGroupId, string userId)
        {
            return await UpdateNotificationGroupSubscriptionAsync(userGroupId, userId, true, true);
        }

        public async Task<DbActionResult> UnsubscribeFromNotificationGroupAsync(string userGroupId, string userId)
        {
            return await UpdateNotificationGroupSubscriptionAsync(userGroupId, userId, false, false);
        }

        public async Task<DbActionResult> SaveUserGroupMembershipAsync(UserGroupMembership membership)
        {
            var result = DbActionResult.Error;
            var oldMembership = await _connectDb.UserGroupMemberships
                .Where(x => (x.UserGroupId == membership.UserGroupId) && (x.UserId == membership.UserId))
                .FirstOrDefaultAsync();
            if (oldMembership == null)
            {
                membership.AddedDT = DateTime.Now;
                membership.AddedBy = _userContext.UserId;
                _connectDb.UserGroupMemberships.Add(membership);
                result = DbActionResult.Insert;
            }
            else
            {
                oldMembership.AccessLevel = membership.AccessLevel;
                oldMembership.AllowEmailMessaging = membership.AllowEmailMessaging;
                oldMembership.AllowSmsMessaging = membership.AllowSmsMessaging;
                result = DbActionResult.Update;
            }
            await _connectDb.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteUserGroupMembershipAsync(string userGroupId, string userId)
        {
            var member = await _connectDb.UserGroupMemberships
                .Where(x => ((x.UserGroupId == userGroupId) && (x.UserId == userId)))
                .FirstOrDefaultAsync();

            if (member == null)
                return false;

            _connectDb.UserGroupMemberships.Remove(member);
            await _connectDb.SaveChangesAsync();
            return true;
        }

        public async Task UpdateGroupMemberships(string userId, IList<UserGroupMembership> addMemberships, IList<UserGroupMembership> removeMemberships)
        {
            // Delete the memberships
            var deleteIds = removeMemberships.Select(m => m.UserGroupId).ToList();
            var deleteList = await _connectDb.UserGroupMemberships
                            .Where(m => deleteIds.Contains(m.UserGroupId) && (m.UserId == userId))
                            .ToListAsync();
            _connectDb.UserGroupMemberships.RemoveRange(deleteList);

            foreach (var addMembership in addMemberships)
            {
                var existingMembership = await _connectDb.UserGroupMemberships
                            .Where(m => (m.UserGroupId == addMembership.UserGroupId) && (m.UserId == addMembership.UserId))
                            .FirstOrDefaultAsync();
                if (existingMembership == null)
                {
                    // Add the new membership
                    addMembership.AccessLevel = Identity.Models.AccessLevel.Audience;
                    addMembership.AddedDT = DateTime.Now;
                    addMembership.AddedBy = _userContext.UserId;
                    _connectDb.UserGroupMemberships.Add(addMembership);
                }
                else
                {
                    // Update the existing memeberships
                    existingMembership.AllowEmailMessaging = addMembership.AllowEmailMessaging;
                    existingMembership.AllowSmsMessaging = addMembership.AllowSmsMessaging;
                }
            }

            await _connectDb.SaveChangesAsync();
        }

        public async Task<IEnumerable<GroupClaim>> GetGroupClaimsAsync(string groupId)
        {

            return await _identityDb.GroupClaims.Where(x => x.GroupId == groupId).ToListAsync();
        }

        public async Task<IEnumerable<GroupClaim>> GetGroupClaimsAsync(string groupId, string claimType)
        {
            Ensure.Argument.NotNull(groupId);
            Ensure.Argument.NotNull(claimType);
            return await _identityDb.GroupClaims.Where(x => x.GroupId == groupId && x.ClaimType == claimType).ToListAsync();
        }

        public async Task<IEnumerable<GroupClaim>> GetGroupClaimsAsync(Claim claim)
        {
            Ensure.Argument.NotNull(claim);
            
            return await _identityDb.GroupClaims.Where(x => x.ClaimType == claim.Type && x.ClaimValue == claim.Value).ToListAsync();
        }


        public async Task<IdentityResult> AddClaimAsync(string groupId, Claim claim, string claimScope = null)
        {
            Ensure.Argument.NotNull(groupId);
            Ensure.Argument.NotNull(claim);

            IdentityResult result;
            try
            {
                _identityDb.GroupClaims.Add(new GroupClaim()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    GroupId = groupId,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });

                await _identityDb.SaveChangesAsync();

                result = IdentityResult.Success;
            }
            catch (Exception ex)
            {
                result = IdentityResult.Failed(new IdentityError() { Description = ex.Message });
            }

            return result;
        }

        public async Task<IdentityResult> RemoveClaimAsync(string claimId)
        {
            {
                Ensure.Argument.Is(claimId != "", "claimId is required");
                IdentityResult result;

                var claim = await _identityDb.GroupClaims.FirstOrDefaultAsync(x => x.Id == claimId);

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
                        _identityDb.GroupClaims.Remove(claim);
                        await _identityDb.SaveChangesAsync();

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
