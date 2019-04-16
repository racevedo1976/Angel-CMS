using Angelo.Identity.Models;
using System.Collections.Generic;
using System.Security.Claims;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Connect.Security
{
    public class SecurityClaimConfigViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<RoleClaim> SelectedRoles { get; set; }
        public IEnumerable<string> SelectedUsers { get; set; }
        public IEnumerable<string> SelectedGroups { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string RolesLabel { get; set; } = "Allowed Roles";
        public string UsersLabel { get; set; } = "Allowed Users";
        public string GroupsLabel { get; set; } = "Allowed Groups";
        public bool AllowUsers { get; set; }
        public bool AllowRoles { get; set; }
        public bool AllowGroups { get; set; }

        public Claim Claim { get; set; }
        public string ResourceType { get; set; }

    }
}
