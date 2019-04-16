using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IGroupProvider
    {
        UserGroupType GroupType { get; } // the title of the group, eg. Connection Group, Notification Group, Site Role
        Task<IEnumerable<IGroup>> GetGroups(); // can take dependency on UserContext if needed to scope to current user
        Task<IEnumerable<IGroup>> GetUserGroups(); // can take dependency on UserContext if needed to scope to current user. Get User Group Type.
    }
}
