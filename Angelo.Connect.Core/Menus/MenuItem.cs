using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Configuration;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;

namespace Angelo.Connect.Menus
{
    public abstract class MenuItem : IMenuItem
    {
        
        public string Title { get; set; }
        public string Url { get; set; }
        public IconType Icon { get; set; }
        public bool Active { get; set; }
        public int SortOrder { get; set; }

        public IEnumerable<IMenuItem> MenuItems { get; set; }

        public MenuItem()
        {
            MenuItems = new List<IMenuItem>();
        }

        public abstract bool Authorize(UserContext user);
    }
}
