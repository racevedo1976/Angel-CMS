using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class SiteDirectory
    {
        public SiteDirectory()
        {
            Site = new Site();
            Edit = false;
            View = false;
        }

        public string DirectoryId { get; set; }
        public string SiteId { get; set; }
        public bool View { get; set; }
        public bool Edit { get; set; }
        public Site Site { get; set; }

    }
}
