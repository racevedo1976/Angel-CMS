using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class UserGroup: IGroup
    {
        public UserGroup()
        {
            Memberships = new List<UserGroupMembership>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public DateTime CreatedDT { get; set; }
        public string CreatedBy { get; set; }
        public UserGroupType UserGroupType { get; set; }
        public bool AllowPublicEnrollment { get; set; }

        public List<UserGroupMembership> Memberships { get; set; }
    }
}

