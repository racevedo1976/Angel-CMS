using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class GroupMembership
    {
        public string Id { get; set; }
        public string GroupId { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public Group Group { get; set; }
    }
}
