using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Kendo.Mvc.Extensions;

using Angelo.Common.Extensions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using Angelo.Identity.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.Controllers.Api
{

    [Route("/sys/corp/api/sites")]
    public class SiteSettingsDataController : BaseController
    {
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;
        private DirectoryManager _directoryManager;

        public SiteSettingsDataController(PageManager pageManager, SiteContext siteContext, 
            SiteManager siteManager, ILogger<SecurityPoolManager> logger,
            ClientManager clientManager, IOptions<RequestLocalizationOptions> localizationOptions,
            SiteTemplateManager templateManager,
            DirectoryManager directoryManager) : base(logger)
            {
            _pageManager = pageManager;
            _siteManager = siteManager;
            _clientManager = clientManager;
            _localizationOptions = localizationOptions;
            _directoryManager = directoryManager;
        }


        [Authorize(policy: PolicyNames.SiteSettingsEdit)]
        [HttpPost, Route("/sys/sites/{tenant}/api/settings")]
        public async Task<ActionResult> SaveSiteSettings(string siteId, SiteSettingsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var dictionary = model.ToDictionary();
                dictionary["GoogleTrackingId"] = dictionary["GoogleTrackingId"] ?? string.Empty;
                var settings = dictionary.ConvertTo<SiteSetting>(x => new SiteSetting { FieldName = x.Key, Value = x.Value.ToString() });

                await _siteManager.SaveSiteSettingsAsync(siteId, settings);

                return Ok(model);
            }

            return ErrorView(ModelState);
        }
    }
}
