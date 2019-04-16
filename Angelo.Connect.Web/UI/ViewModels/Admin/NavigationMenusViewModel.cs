using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NavigationMenusViewModel
    {
        public NavigationMenusViewModel()
        {
            NavigationMenus = new List<NavigationMenuViewModel>();
            SelectedNavigationMenu = new NavigationMenuViewModel();
        }

        public ICollection<NavigationMenuViewModel> NavigationMenus { get; set; }
        public NavigationMenuViewModel SelectedNavigationMenu { get; set; }
    }
}
