using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using AutoMapper.Extensions;

using Angelo.Identity;
using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Security;
using Angelo.Identity.Services;

namespace Angelo.Connect.Web.UI.Controllers.Api
{

    //[Route("/sys/clients/{tenant}/api")]
    public class ClientAppsDataController : BaseController
    {
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;
        private SiteTemplateManager _templateManager;
        private DirectoryManager _directoryManager;

        public ClientAppsDataController(PageManager pageManager, SiteContext siteContext, 
            SiteManager siteManager, ILogger<SecurityPoolManager> logger,
            ClientManager clientManager, IOptions<RequestLocalizationOptions> localizationOptions,
            SiteTemplateManager templateManager,
            DirectoryManager directoryManager) : base(logger)
            {
            _pageManager = pageManager;
            _siteManager = siteManager;
            _clientManager = clientManager;
            _localizationOptions = localizationOptions;
            _templateManager = templateManager;
            _directoryManager = directoryManager;
        }

        private List<SiteViewModel> ProjectToSiteViewModel(IEnumerable<Site> sites)
        {
            var model = sites.ProjectTo<SiteViewModel>().ToList();
            var cultures = _localizationOptions.Value.SupportedCultures;
            foreach (var vm in model)
            {
                vm.DefaultCultureDisplayName = cultures.Where(x => x.Name == vm.DefaultCultureKey).FirstOrDefault()?.DisplayName;
                vm.Status = (vm.Published) ? "Published" : "Hidden";
            }
            return model;
        }

        [Authorize(policy: PolicyNames.ClientSitesRead)]
        [HttpPost, Route("/sys/clients/{tenant}/api/sites/data")]
        public async Task<JsonResult> GetClientSites([DataSourceRequest] DataSourceRequest request, string appId)
        {
            var sites = await _siteManager.GetAppSitesAsync(appId);
            var model = ProjectToSiteViewModel(sites);
            model = model.OrderBy(x => x.ClientName).ThenBy(x => x.Title).ToList();
            var result = model.AsQueryable().ToDataSourceResult(request);
            return Json(result);
        }


    }
}
