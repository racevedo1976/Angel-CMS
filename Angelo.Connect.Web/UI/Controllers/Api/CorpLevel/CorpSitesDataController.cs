using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using AutoMapper.Extensions;

using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using Angelo.Connect.Extensions;
using Angelo.Identity.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.Controllers.Api
{

    [Route("/sys/corp/api/sites")]
    public class CorpSitesDataController : BaseController
    {
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private SitePublisher _sitePublisher;
        private ClientManager _clientManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;
        private DirectoryManager _directoryManager;

        public CorpSitesDataController
        (
            PageManager pageManager, 
            SiteContext siteContext, 
            SiteManager siteManager,
            SitePublisher sitePublisher,
            SiteTemplateManager templateManager,
            DirectoryManager directoryManager,
            ClientManager clientManager, 
            IOptions<RequestLocalizationOptions> localizationOptions,
            ILogger<SecurityPoolManager> logger

        ) : base(logger)
        {
            _pageManager = pageManager;
            _siteManager = siteManager;
            _sitePublisher = sitePublisher;
            _clientManager = clientManager;
            _localizationOptions = localizationOptions;
            _directoryManager = directoryManager;
        }

        private List<SiteViewModel> ProjectToSiteViewModel(IEnumerable<Site> sites)
        {
            var model = new List<SiteViewModel>();
            var cultures = _localizationOptions.Value.SupportedCultures;
            foreach (var site in sites)
            {
                var vm = site.ProjectTo<SiteViewModel>();
                vm.DefaultCultureDisplayName = cultures.Where(x => x.Name == site.DefaultCultureKey).FirstOrDefault()?.DisplayName;
                vm.Status = vm.Published ? "Published" : "Hidden";
                model.Add(vm);
            }
            return model;
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpPost, Route("data")]
        public async Task<JsonResult> GetSites([DataSourceRequest] DataSourceRequest request)
        {
            var sites = await _siteManager.GetCorpSitesAsync();
            var model = ProjectToSiteViewModel(sites);
            model = model.OrderBy(x => x.ClientName).ThenBy(x => x.Title).ToList();
            var result = model.AsQueryable().ToDataSourceResult(request);
            return Json(result);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpPost, Route("")]
        public async Task<ActionResult> CreateSite(SiteCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.TenantKey = model.TenantKey.FormatTenantKey();

                if (await ValidateDuplicateSiteTitleAsync(model.ClientId, model.Title, "") == false)
                    return BadRequest(ModelState);

                if (await ValidateDuplicateSiteTenantKeyAsync(model.ClientId, model.TenantKey, "") == false)
                    return BadRequest(ModelState);

                var site = new Site()
                {
                    ClientId = model.ClientId,
                    ClientProductAppId = model.ClientProductAppId,
                    Title = model.Title,
                    TenantKey = model.TenantKey,
                    SiteTemplateId = model.TemplateId,
                    SecurityPoolId = model.UserPoolKey
                };
                site.DefaultCultureKey = _localizationOptions.Value.DefaultRequestCulture.Culture.Name;
                site.Cultures.Add(new SiteCulture() { CultureKey = site.DefaultCultureKey });

                // Create & Initialize Site
                await _siteManager.CreateAsync(site);
                await _sitePublisher.CreateInitialVersion(site);

                // Update model values from newly created site
                model.Id = site.Id;
                model.UserPoolKey = site.SecurityPoolId;

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpPost, Route("{siteId}")]
        public async Task<ActionResult> SiteUpdate(string siteId, SiteViewModel model)
        {
            if (ModelState.IsValid)
            {
                model.TenantKey = model.TenantKey.FormatTenantKey();

                if (ValidateSiteIdInRouteAndModel(siteId, model.Id) == false)
                    return BadRequest(ModelState);

                if (await ValidateDuplicateSiteTitleAsync(model.ClientId, model.Title, model.Id) == false)
                    return BadRequest(ModelState);

                if (await ValidateDuplicateSiteTenantKeyAsync(model.ClientId, model.TenantKey, model.Id) == false)
                    return BadRequest(ModelState);

                var site = model.ProjectTo<Site>();
                site.Cultures = model.Cultures.Where(x => x.IsSelected).Select(x => new SiteCulture() { CultureKey = x.CultureKey }).ToList();
                site.SiteDirectories = site.SiteDirectories.Where(d => 
                                                    model.SiteDirectories.Where(y => y.Selected).Select(x => x.DirectoryId).ToList().Contains(d.DirectoryId)).ToList();

                await _siteManager.UpdateAsync(site);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpDelete, Route("{id}")]
        public async Task<ActionResult> DeleteSite(string id)
        {
            await _siteManager.DeleteAsync(id);
            return Json(id);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpPost, Route("{id}/domains")]
        public async Task<ActionResult> InsertSiteDomain(string id, CorpSiteDomainViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ValidateSiteIdInRouteAndModel(id, model.SiteId) == false)
                    return BadRequest(ModelState);

                await _siteManager.AddSiteDomainAsync(model.SiteId, model.DomainKey, model.IsDefault);

                if (model.IsDefault)
                {
                    var site = await _siteManager.GetByIdAsync(id);
                    await _sitePublisher.QueueSearchIndex(site);
                }

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpPost, Route("{id}/domains/0")]
        public async Task<ActionResult> UpdateSiteDomain(string id, CorpSiteDomainViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (ValidateSiteIdInRouteAndModel(id, model.SiteId) == false)
                    return BadRequest(ModelState);

                await _siteManager.RemoveSiteDomainAsync(model.SiteId, model.OriginalDomainKey);
                await _siteManager.AddSiteDomainAsync(model.SiteId, model.DomainKey, model.IsDefault);

                if(model.IsDefault)
                {
                    var site = await _siteManager.GetByIdAsync(id);
                    await _sitePublisher.QueueSearchIndex(site);
                }

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpDelete, Route("{id}/domains/0")]
        public async Task<ActionResult> DeleteSiteDomain(string id, CorpSiteDomainViewModel model)
        {
            if (model != null)
            {
                if (ValidateSiteIdInRouteAndModel(id, model.SiteId) == false)
                    return BadRequest(ModelState);

                await _siteManager.RemoveSiteDomainAsync(model.SiteId, model.OriginalDomainKey);
                return Json(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        [HttpPost, Route("{id}/collections")]
        public async Task<ActionResult> SaveSiteCollections(SiteCollectionMapViewModel model)
        {
            if (model != null)
            {
                var removeIds = model.Items.Where(m => (m.Selected == false) && (m.OriginalStatus != m.Selected)).Select(i => i.Id).ToList();
                await _siteManager.RemoveSiteCollectionsAsync(model.SiteId, removeIds);

                var addIds = model.Items.Where(m => (m.Selected == true) && (m.OriginalStatus != m.Selected)).Select(i => i.Id).ToList();
                await _siteManager.AddSiteCollectionsAsync(model.SiteId, addIds);

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        // Verify that the site id in the route and model match.
        protected bool ValidateSiteIdInRouteAndModel(string routeId, string modelId)
        {
            if (routeId != modelId)
            {
                ModelState.AddModelError("Id", "Site Id in route and model do not match.");
                return false;
            }
            return true;
        }

        // Verify that the site title is unique.
        protected async Task<bool> ValidateDuplicateSiteTitleAsync(string clientId, string title, string siteId)
        {
            var foundSite = await _siteManager.GetByTitleAsync(clientId, title);
            if (foundSite != null)
            {
                if (foundSite.Id != siteId)
                {
                    var duplicateTitleErrorMessage = Localize("UI.Views.Admin.Sites", "Title.Error.Duplicate");
                    ModelState.AddModelError("Title", duplicateTitleErrorMessage);
                    return false;
                }
            }
            return true;
        }

        // Verify that the site tenant key is unique.
        protected async Task<bool> ValidateDuplicateSiteTenantKeyAsync(string clientId, string tenantKey, string siteId)
        {
            var foundSite = await _siteManager.GetByTenantKeyAsync(clientId, tenantKey);
            if (foundSite != null)
            {
                if (foundSite.Id != siteId)
                {
                    var duplicateTenantKeyErrorMessage = Localize("UI.Views.Admin.Sites", "TenantKey.Error.Duplicate");
                    ModelState.AddModelError("TenantKey", duplicateTenantKeyErrorMessage);
                    return false;
                }
            }
            return true;
        }


    }
}
