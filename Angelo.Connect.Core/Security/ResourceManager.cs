using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Microsoft.EntityFrameworkCore;
using Angelo.Identity.Models;

namespace Angelo.Connect.Security
{
    public class ResourceManager
    {
        private ConnectDbContext _db;

        public ResourceManager(ConnectDbContext db)
        {
            _db = db;
        }

        //public async Task<IEnumerable<ResourceClaim>> GetResourceClaimsAsync(IDocument resource)
        //{
        //    return await _db.ResourceClaims.Where(x => x.ResourceId == resource.DocumentId
        //                                        && x.ResourceType == resource.GetType().Name).ToListAsync();
        //}

        //public async Task<IEnumerable<ResourceClaim>> GetResourceDocumentClaimsAsync(IDocument resource)
        //{
        //    return await _db.ResourceClaims.Where(x => x.ResourceId == resource.DocumentId
        //                                        && x.ResourceType == resource.GetType().Name).ToListAsync();
        //}

        //public async Task<IEnumerable<ResourceClaim>> GetUserClaims(User user)
        //{
        //    return await _db.ResourceClaims.Where(x => x.UserId == user.Id).ToListAsync();
        //}

        //public async Task<IEnumerable<ResourceClaim>> GetUserFolderClaims(User user, IDocument resource)
        //{
        //    return await _db.ResourceClaims.Where(x => x.UserId == user.Id
        //                                            && x.ResourceType == resource.GetType().Name).ToListAsync();
        //}

        public IQueryable<ResourceClaim> Query()
        {
            return _db.ResourceClaims.AsQueryable();
        }

        public async Task<IEnumerable<ResourceClaim>> GetResourceClaimsAsync<IDocument>(string userId)
        {
            return await _db.ResourceClaims.Where(x => x.UserId == userId
                                                    && x.ResourceType == typeof(IDocument).Name).ToListAsync();
        }

        public async Task<IEnumerable<ResourceClaim>> GetResourceClaimsByUserAsync(string userId)
        {
            return await _db.ResourceClaims.Where(x => x.UserId == userId).ToListAsync();
        }

        public async Task<IEnumerable<ResourceClaim>> GetResourceClaimsAsync(string userId, string resourceType)
        {
            return await _db.ResourceClaims.Where(x => x.UserId == userId
                                                    && x.ResourceType == resourceType).ToListAsync();
        }

        public async Task<IEnumerable<ResourceClaim>> GetResourceClaimsAsync(string userId, string resourceType, string resourceId)
        {
            return await _db.ResourceClaims.Where(x => x.UserId == userId
                                                    && x.ResourceType == resourceType
                                                    && x.ResourceId == resourceId).ToListAsync();
        }

        public async Task<IEnumerable<ResourceClaim>> GetAllResourceClaimsAsync(string resourceType, string resourceId)
        {
            return await _db.ResourceClaims.Where(x => x.ResourceType == resourceType
                                                    && x.ResourceId == resourceId).ToListAsync();
        }

        public async Task<ResourceClaim> AddResourceClaimAsync(string resourceId, string resourceType, string userId, string claimType)
        {
            if (_db.ResourceClaims.Any(x => x.ResourceId == resourceId && x.ResourceType == resourceType && x.UserId == userId && x.ClaimType == claimType))
                return null;

            var resourceClaim = new ResourceClaim
            {
                ResourceId = resourceId,
                ResourceType = resourceType,
                UserId = userId,
                ClaimType = claimType
            };

            _db.ResourceClaims.Add(resourceClaim);

            await _db.SaveChangesAsync();

            return resourceClaim;
        }

        public async Task<bool> RemoveResourceClaimAsync(string resourceId, string resourceType, string userId, string claimType)
        {
            var claim = await _db.ResourceClaims.FirstOrDefaultAsync(x => x.ResourceId == resourceId && x.ResourceType == resourceType && x.UserId == userId && x.ClaimType == claimType);
            if (claim == null)
                return false;
            else
            {
                _db.ResourceClaims.Remove(claim);
                await _db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<GroupResourceClaim> AddGroupResourceClaimAsync(string resourceId, string resourceType, string groupId, string groupType, string claimType)
        {
            if (_db.GroupResourceClaims.Any(x => x.ResourceId == resourceId 
                                            && x.ResourceType == resourceType
                                            && x.GroupId == groupId 
                                            && x.GroupProviderType == groupType
                                            && x.ClaimType == claimType))
                return null;

            var resourceClaim = new GroupResourceClaim
            {
                ResourceId = resourceId,
                ResourceType = resourceType,
                GroupId = groupId,
                GroupProviderType = groupType,
                ClaimType = claimType
            };

            _db.GroupResourceClaims.Add(resourceClaim);

            await _db.SaveChangesAsync();

            return resourceClaim;
        }

        public async Task<bool> RemoveGroupResourceClaimAsync(string resourceId, string resourceType, string groupId, string groupType, string claimType)
        {
            var claim = await _db.GroupResourceClaims.FirstOrDefaultAsync(x => x.ResourceId == resourceId 
                                                                       && x.ResourceType == resourceType 
                                                                       && x.GroupId == groupId
                                                                       && x.GroupProviderType == groupType
                                                                       && x.ClaimType == claimType);
            if (claim == null)
                return false;
            else
            {
                _db.GroupResourceClaims.Remove(claim);
                await _db.SaveChangesAsync();
                return true;
            }
        }

        //methods for user Groups.

        public async Task<IEnumerable<GroupResourceClaim>> GetGroupResourceClaimsAsync(string groupType, string groupId)
        {
            return await _db.GroupResourceClaims.Where(x => x.GroupId == groupId 
                                                    && x.GroupProviderType == groupType).ToListAsync(); 
        }

        public async Task<IEnumerable<GroupResourceClaim>> GetGroupResourceClaimsAsync(string groupType, string groupId, string resourceType)
        {
            return await _db.GroupResourceClaims.Where(x => x.GroupId == groupId
                                                    && x.GroupProviderType == groupType
                                                    && x.ResourceType == resourceType).ToListAsync();
        }

        public async Task<IEnumerable<GroupResourceClaim>> GetGroupResourceClaimsAsync(string groupType, string groupId, string resourceType, string resourceId)
        {
            return await _db.GroupResourceClaims.Where(x => x.GroupId == groupId
                                                    && x.GroupProviderType == groupType
                                                    && x.ResourceType == resourceType
                                                    && x.ResourceId == resourceId).ToListAsync();
        }

        public async Task<IEnumerable<GroupResourceClaim>> GetAllGroupResourceClaimsAsync(string resourceType, string resourceId)
        {
            return await _db.GroupResourceClaims.Where(x => x.ResourceId == resourceId
                                                         && x.ResourceType == resourceType).ToListAsync();
        }





        //public void AddUser(IResource resource, string claimType, UserMembership user)
        //{
        //    var claim = BuildClaim(resource, claimType);
        //    var userClaim = BuildUserClaim(user, claim);

        //    _db.UserClaims.Add(userClaim);
        //    _db.SaveChanges();
        //}

        //public void AddGroup(IResource resource, string claimType, Role group)
        //{
        //    var claim = BuildClaim(resource, claimType);
        //    var groupClaim = BuildGroupClaim(group, claim);

        //    _db.RoleClaims.Add(groupClaim);
        //    _db.SaveChanges();
        //}

        //public void RemoveUser(IResource resource, string claimType, UserMembership user)
        //{
        //    var claim = BuildClaim(resource, claimType);
        //    var userClaim = BuildUserClaim(user, claim);

        //    _db.UserClaims.Remove(userClaim);
        //    _db.SaveChanges();
        //}

        //public void RemoveGroup(IResource resource, string claimType, Role group)
        //{
        //    var claim = BuildClaim(resource, claimType);
        //    var groupClaim = BuildGroupClaim(group, claim);

        //    _db.RoleClaims.Remove(groupClaim);
        //    _db.SaveChanges();
        //}

        //public void SetUsers(IResource resource, string claimType, IEnumerable<UserMembership> users)
        //{

        //}

        //public void SetGroups(IResource resource, string claimType, IEnumerable<string> groupIds)
        //{

        //}

        //public IEnumerable<Role> GetGroups(IResource resource, string claimType)
        //{
        //    var claim = BuildClaim(resource, claimType);

        //    //_roleManager.get

        //    return null;
        //}

        //public IEnumerable<User> GetUsers(IResource resource, string claimType, bool inheritRoleClaims = false)
        //{
        //    var claim = BuildClaim(resource, claimType);

        //    return null;
        //}

        //private Claim BuildClaim(IResource resource, string claimType)
        //{
        //    return new Claim(resource.GetType().FullName + "/" + claimType, resource.Id);
        //}

        //private RoleClaim BuildGroupClaim(Role group, Claim claim)
        //{
        //    return new RoleClaim
        //    {
        //        ClaimType = claim.Type,
        //        ClaimValue = claim.Value,
        //        RoleId = group.Id
        //    };
        //}

        //private UserClaim BuildUserClaim(UserMembership user, Claim claim)
        //{
        //    return new UserClaim
        //    {
        //        ClaimType = claim.Type,
        //        ClaimValue = claim.Value,
        //        UserId = user.UserId
        //    };
        //}
    }
}
