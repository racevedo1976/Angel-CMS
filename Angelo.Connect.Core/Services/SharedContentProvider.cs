using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Services
{
    public class SharedContentProvider : ISharedContent
    {
        private ResourceManager _resourceManager;

        public SharedContentProvider(ResourceManager resourceManager)
        {
            _resourceManager = resourceManager;
        }
        public Task<IEnumerable<GroupResourceClaim>> GetGroupSharedContent(string groupId, string resourceType, string groupType)
        {
            return _resourceManager.GetGroupResourceClaimsAsync(groupType, groupId, resourceType);
        }

        public Task<IEnumerable<GroupResourceClaim>> GetGroupSharedContent<IGroupProvider>(string groupId)
        {
            throw new NotImplementedException();
            //return _resourceManager.GetGroupResourcesClaims(groupId);
        }

        public Task<IEnumerable<ResourceClaim>> GetSharedContent(string userId, string resourceType)
        {
            return _resourceManager.GetResourceClaimsAsync(userId, resourceType);
           
        }

        public Task<IEnumerable<ResourceClaim>> GetSharedContent<IContentType>(string userId)
        {
            return _resourceManager.GetResourceClaimsAsync(userId, typeof(IContentType).Name);
        }

        public async Task<bool> IsAnythingShared(string userId, string resourceType)
        {
            return (await _resourceManager.GetResourceClaimsAsync(userId, resourceType)).Any();
        }
    }
}
