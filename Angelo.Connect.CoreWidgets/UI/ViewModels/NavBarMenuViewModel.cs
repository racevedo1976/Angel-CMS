using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.CoreWidgets.UI.ViewModels
{
    public class NavBarMenuViewModel
    {
        public string Id { get; set; }
        public string NavMenuId { get; set; }

        public IEnumerable<NavigationMenu> NavMenus { get; set; }
    }
}
