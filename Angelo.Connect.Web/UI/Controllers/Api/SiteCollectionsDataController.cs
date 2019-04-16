using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class SiteCollectionsDataController : Controller
    {
        private SiteCollectionManager _siteCollectionManager;

        public SiteCollectionsDataController(SiteCollectionManager siteCollectionManager)
        {
            _siteCollectionManager = siteCollectionManager;
        }

        [Authorize]
        [HttpPost, Route("/api/siteCollections/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string clientId)
        {
            var siteCollections = await _siteCollectionManager.GetSiteCollectionsAsync(clientId);
            var model = siteCollections.ProjectTo<SiteCollectionViewModel>();
            return Json(model.ToDataSourceResult(request));
        }

        [Authorize]
        [HttpPost, Route("/api/siteCollections")]
        public async Task<JsonResult> Create(SiteCollectionViewModel vm)
        {
            var siteCollection = new SiteCollection();
            var newId = "";
            siteCollection.Name = vm.Name;
            siteCollection.ClientId = vm.ClientId;

            newId = await _siteCollectionManager.CreateSiteCollectionAsync(siteCollection);
            siteCollection.Id = newId;

            return Json(siteCollection);
        }

        [Authorize]
        [HttpPost, Route("/api/siteCollections/nonSites")]
        public async Task<JsonResult> NonSites([DataSourceRequest]DataSourceRequest request, string clientId, string collectionId)
        {
            var sites = await _siteCollectionManager.GetNonSitesAsync(collectionId, clientId);
            List<SiteCollectionSiteViewModel> model;

            if (sites.Count > 0)
            {
                model = new List<SiteCollectionSiteViewModel>();

                foreach (var site in sites)
                {
                    var temp = new SiteCollectionSiteViewModel();
                    temp.SiteId = site.Id;
                    temp.ClientId = clientId;
                    temp.SiteCollectionId = collectionId;
                    temp.Title = site.Title;
                    model.Add(temp);
                }

                return Json(model.ToDataSourceResult(request));
            }
            else
            {
                model = new List<SiteCollectionSiteViewModel>();
                return Json(model.ToDataSourceResult(request));
            }
        }

        [Authorize]
        [HttpPost, Route("/api/siteCollections/addSite")]
        public async Task<JsonResult> AddSite([DataSourceRequest]DataSourceRequest request, SiteCollectionSiteViewModel vm)
        {
            var result = await _siteCollectionManager.SitesAddNewAsync(vm.SiteCollectionId, vm.SiteId);

            return Json(Ok());
        }

        [Authorize]
        [HttpPut, Route("/api/siteCollections")]
        public async Task<JsonResult> Edit(SiteCollectionViewModel vm)
        {
            var siteCollection = new SiteCollection();
            siteCollection.Id = vm.Id;
            siteCollection.Name = vm.Name;
            siteCollection.ClientId = vm.ClientId;

            var isUpdated = await _siteCollectionManager.UpdateSiteCollectionDetailsAsync(siteCollection);

            return Json(siteCollection);
        }

        [Authorize]
        [HttpDelete, Route("/api/siteCollections/deleteSite")]
        public async Task<JsonResult> DeleteSite([DataSourceRequest]DataSourceRequest request, SiteViewModel vm, string collectionId)
        {
            var siteId = vm.Id;
            var result = await _siteCollectionManager.DeleteSiteFromSiteCollectionAsync(siteId, collectionId);

            return Json(Ok());
        }

        [Authorize]
        [HttpPost, Route("/api/siteCollections/siteList")]
        public async Task<JsonResult> SiteList([DataSourceRequest] DataSourceRequest request, string collectionId)
        {
            var sites = await _siteCollectionManager.GetSiteCollectionSitesAsync(collectionId);
            var model = sites.ProjectTo<SiteViewModel>();
            return Json(model.ToDataSourceResult(request));
        }

    }
}
