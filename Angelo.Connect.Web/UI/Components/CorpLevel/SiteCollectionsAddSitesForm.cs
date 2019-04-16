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
    public class SiteCollectionsAddSitesForm : ViewComponent
    {
        private SiteCollectionManager _siteCollectionManager;

        public SiteCollectionsAddSitesForm(SiteCollectionManager siteCollectionManager)
        {
            _siteCollectionManager = siteCollectionManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId, string collectionId)
        {
            ViewData["ClientId"] = clientId;
            ViewData["CollectionId"] = collectionId;

            if (string.IsNullOrEmpty(collectionId))
            {
                return View();
            }

            return await Task.Run(() => {
                return View();
            });
        }
    }
}
