using System.Security.Claims;

namespace Angelo.Connect.Security
{
    public class SecurityClaimConfig
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public Claim Claim { get; set; }
        public bool AllowUsers { get; set; }
        public string AllowUsersLabel { get; set; } = "Allow these Users";
        public bool AllowRoles { get; set; }
        public string AllowRolesLabel { get; set; } = "Allow these Roles";
        public bool AllowGroups { get; set; }
        public string AllowGroupsLabel { get; set; } = "Allow these Groups";

        public string SecurityPoolId { get; set; }
        public string ResourceType { get; set; }
    }
}
