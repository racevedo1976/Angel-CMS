using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class SiteSetting : BaseSetting
    {
        public string SiteId { get; set; }

        public Site Site { get; set; }
    }
}