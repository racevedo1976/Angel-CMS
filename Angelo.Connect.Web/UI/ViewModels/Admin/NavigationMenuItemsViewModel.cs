using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NavigationMenuItemsViewModel
    {
        public NavigationMenuItemsViewModel()
        {
            NavMenuItems = new List<NavigationMenuItemViewModel>();
            SelectedNavigationMenuItem = new NavigationMenuItemViewModel();
        }

        public string NavigationMenuName { get; set; }

        public ICollection<NavigationMenuItemViewModel> NavMenuItems { get; set; }
        public NavigationMenuItemViewModel SelectedNavigationMenuItem { get; set; }
    }
}
