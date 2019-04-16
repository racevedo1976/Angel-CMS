using System.Collections.Generic;

namespace Angelo.Identity.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public List<GroupMembership> GroupMemberships { get; set; }
    }
}
