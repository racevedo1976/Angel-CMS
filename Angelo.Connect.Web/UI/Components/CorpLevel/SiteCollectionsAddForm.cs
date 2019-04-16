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
    public class SiteCollectionsAddForm : ViewComponent
    {
        private SiteCollectionManager _siteCollectionsManager;

        public SiteCollectionsAddForm(SiteCollectionManager siteCollectionManager)
        {
            _siteCollectionsManager = siteCollectionManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId)
        {
            var model = new SiteCollectionViewModel();
            model.ClientId = clientId;
            return View(model);
        }
    }
}
