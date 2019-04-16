using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteCollectionsEditForm : ViewComponent
    {
        private SiteCollectionManager _siteCollectionsManager;

        public SiteCollectionsEditForm(SiteCollectionManager siteCollectionManager)
        {
            _siteCollectionsManager = siteCollectionManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId, string collectionId = null)
        {
            SiteCollectionViewModel model = null;

            if (collectionId != null)
            {
                var site = await _siteCollectionsManager.GetSiteCollectionByIdAsync(collectionId);
                model = site.ProjectTo<SiteCollectionViewModel>();
                return View(model);
            }
            else
            {
                model = new SiteCollectionViewModel();
                model.ClientId = clientId;
                model.Name = "";

                return View(model);
            }
        }
    }
}
