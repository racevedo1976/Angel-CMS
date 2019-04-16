using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserGroupMultiSelectViewModel
    {
        public UserGroupMultiSelectViewModel()
        {

        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string GroupType { get; set; }

    }

    public static class UserGroupMappings
    {
        public static UserGroupMultiSelectViewModel ToMultiSelectViewModel (UserGroup userGroup, string groupType)
        {
            return new UserGroupMultiSelectViewModel
            {
                Id = $"{groupType}_{userGroup.Id}",
                Name = userGroup.Name,
                GroupType = groupType
            };
        }

        public static IEnumerable<UserGroupMultiSelectViewModel> ToMultiSelectViewModel(IEnumerable<UserGroup> userGroups, string groupType)
        {
            return userGroups.Select(x => ToMultiSelectViewModel(x, groupType)).ToList();
        }
    }
}

    