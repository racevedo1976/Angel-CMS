using System;
using Angelo.Connect.Abstractions;
using Angelo.Identity.Models;
using System.Threading.Tasks;
using Angelo.Connect.Security;
using System.Collections.Generic;
using Angelo.Connect.Models;
using System.Linq;
using Angelo.Identity.Services;
using Kendo.Mvc.Extensions;

namespace Angelo.Connect.Services
{
    public class ConnectionGroupProvider : ISecurityGroupProvider, IGroupProvider
    {
        private GroupManager _groupManager;
        private UserContext _userContext;

        public ConnectionGroupProvider(UserContext userContext, GroupManager groupManager)
        {
            _userContext = userContext;
            _groupManager = groupManager;
        }

        public UserGroupType GroupType
        {
            get
            {
                return UserGroupType.ConnectionGroup;
            }
        }

        public async Task<IEnumerable<IGroup>> GetGroups()
        {
            IList<Group> userGroups = new List<Group>();
            userGroups.AddRange(await _groupManager.GetGroupsOwnedByUser(_userContext.UserId));
            userGroups.AddRange(_groupManager.GetUserMemberships(_userContext.UserId));


            //var groups = _groupManager.GetUserGroupsAssignedToUserWithAccessLevelQuery(_userContext.UserId, accessLevel.ToArray(), UserGroupType.ConnectionGroup).ToList();

            return userGroups.Select(x => new UserGroup
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public async Task<IEnumerable<IGroup>> GetUserGroups()
        {
            IList<Group> userGroups = new List<Group>();
            userGroups.AddRange(await _groupManager.GetGroupsOwnedByUser(_userContext.UserId));
            userGroups.AddRange(_groupManager.GetUserMemberships(_userContext.UserId));


            //var groups = _groupManager.GetUserGroupsAssignedToUserWithAccessLevelQuery(_userContext.UserId, accessLevel.ToArray(), UserGroupType.ConnectionGroup).ToList();

            return userGroups.Select(x => new UserGroup
            {
                Id = x.Id,
                Name = x.Name
            });
        }


        public async Task<bool> IsActiveMember(User user, IGroup group)
        {
            
            throw new NotImplementedException();
        }
    }
}
