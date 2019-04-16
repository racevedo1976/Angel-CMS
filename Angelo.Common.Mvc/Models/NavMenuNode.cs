using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc
{
    public class NavMenuNode
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public int Order { get; set; }
        public string Link { get; set; }
        public bool HasChildren { get; set; }
    }
}

