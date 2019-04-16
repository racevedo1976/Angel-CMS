using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Assignments.Models
{
    public class AssignmentCategory
    {
        public string Id { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }
        public string Title { get; set; }

        public List<AssignmentCategory> Assignments { get; set; }

        public AssignmentCategory()
        {
            Assignments = new List<AssignmentCategory>();
        }
    }
}
