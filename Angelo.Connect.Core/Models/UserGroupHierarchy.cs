using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class UserGroupHierarchy
    {
        public string ParentGroupId { get; set; }
        public string ChildGroupId { get; set; }
        public MembershipType MembershipType { get; set; }
    }
}
