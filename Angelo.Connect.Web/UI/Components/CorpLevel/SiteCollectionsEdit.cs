using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteCollectionsEdit : ViewComponent
    {
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private SiteCollectionManager _siteCollectionManager;

        public SiteCollectionsEdit(SiteManager siteManager, ClientManager clientManager, SiteCollectionManager siteCollectionManager)
        {
            _siteManager = siteManager;
            _clientManager = clientManager;
            _siteCollectionManager = siteCollectionManager;
        }

        protected async Task<SiteCollectionMapViewModel> GetSiteCollectionListAsync(string clientId, string siteId)
        {
            var availableSiteCollections = await _siteCollectionManager.GetSiteCollectionsAsync(clientId);
            var assignedSiteCollections = await _siteManager.GetSiteCollectionsAsync(siteId);
            var siteCollectionMap = new SiteCollectionMapViewModel()
            {
                SiteId = siteId,
                ClientId = clientId
            };
            foreach (var item in availableSiteCollections)
            {
                var newItem = new SiteCollectionMapViewModel.Item();
                newItem.Id = item.Id;
                newItem.Name = item.Name;
                if (assignedSiteCollections.Where(x => x.Id == item.Id).Any())
                {
                    newItem.Selected = true;
                    newItem.OriginalStatus = true;
                }
                else
                {
                    newItem.Selected = false;
                    newItem.OriginalStatus = false;
                }
                siteCollectionMap.Items.Add(newItem);
            }
            return siteCollectionMap;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = "", string siteId = "")
        {
            if (string.IsNullOrEmpty(siteId))
                return new ViewComponentPlaceholder();

            var model = await GetSiteCollectionListAsync(clientId, siteId);
            return View("SiteCollectionsEdit", model);
        }

    }
}

