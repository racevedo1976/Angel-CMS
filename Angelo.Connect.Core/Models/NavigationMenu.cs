using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class NavigationMenu
    {
        public NavigationMenu()
        {
            MenuItems = new List<NavigationMenuItem>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string SiteId { get; set; }  // FK for Site Id

        public string Scope { get; set; } // For finding menus by purpose (primary site menu, etc)

        public bool IsDefault            // True if the menu is the main site menu
        {
            get {
                return Scope.ToLower() == "main";
            }
        }

        public Site Site { get; set; }

        public ICollection<NavigationMenuItem> MenuItems { get; set; }
    }
}
