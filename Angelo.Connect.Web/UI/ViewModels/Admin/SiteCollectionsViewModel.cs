using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteCollectionsViewModel
    {
        public SiteCollectionsViewModel()
        {
            SiteCollections = new List<SiteCollectionViewModel>();
            SelectedSiteCollection = new SiteCollectionViewModel();
        }

        public ICollection<SiteCollectionViewModel> SiteCollections { get; set; }
        public SiteCollectionViewModel SelectedSiteCollection { get; set; }
    }
}
