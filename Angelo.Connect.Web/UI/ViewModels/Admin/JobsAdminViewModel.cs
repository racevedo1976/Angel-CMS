using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Logging;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class JobsAdminViewModel
    {
        public DbLogEvent Job { get; set; }
        public List<DbLogEvent> Jobs { get; set; }

        public JobsAdminViewModel()
        {
            Job = new DbLogEvent();
            Jobs = new List<DbLogEvent>();
        }
    }
}
