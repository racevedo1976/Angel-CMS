using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.NavMenu.Data
{
    public class NavMenuViewItem
    {
        public NavMenuViewItem()
        {
            Children = new List<NavMenuViewItem>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string Target { get; set; }


        public List<NavMenuViewItem> Children { get; set; }
    }
}


