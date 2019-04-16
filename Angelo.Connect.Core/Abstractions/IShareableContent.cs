using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface ISharedContent
    {
        Task<bool> IsAnythingShared(string userId, string resourceType);

        Task<IEnumerable<ResourceClaim>> GetSharedContent(string userId, string resourceType);
        Task<IEnumerable<ResourceClaim>> GetSharedContent<IContentType>(string userId);

        Task<IEnumerable<GroupResourceClaim>> GetGroupSharedContent(string groupId, string resourceType, string groupType);
        Task<IEnumerable<GroupResourceClaim>> GetGroupSharedContent<IGroupProvider>(string groupId);

        

    }
}
