using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class NavigationMenuItem
    {
        public NavigationMenuItem()
        {
            Children = new List<NavigationMenuItem>();
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string NavMenuId { get; set; }  // FK for Navigation Menu ID
        public string ParentId { get; set; }  // If the item is part of a submenu, this is the FK to the Parent Navigation Menu Item
        public string ContentType { get; set; } // see NavigationMenuItemType or The INavContentProvider.Name
        public bool TargetType { get; set; }
        public int Order { get; set; }  // This is the position for the menu item within the navigation menu
        public string ExternalURL { get; set; }  // This is only for links that point to an External URL
        public string ContentId { get; set; }  // Used to get the menu item from the provider

        public NavigationMenu NavMenu { get; set; }
        public NavigationMenuItem Parent { get; set; }
        public ICollection<NavigationMenuItem> Children {get;set;}
    }
}
