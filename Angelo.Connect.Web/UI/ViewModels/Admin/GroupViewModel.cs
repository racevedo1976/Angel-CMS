using Angelo.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class GroupViewModel
    {
       
        public string Id { get; set; }
        public string Name { get; set; }
        public string OwnerId { get; set; }
        public List<GroupMembership> GroupMemberships { get; set; }
        
    }
}
