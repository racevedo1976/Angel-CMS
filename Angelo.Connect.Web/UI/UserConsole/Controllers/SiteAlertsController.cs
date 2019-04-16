using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Microsoft.AspNetCore.Authorization;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Web.UI.Controllers.Public
{
    [RequireFeature(FeatureId.Alerts)]
    public class SiteAlertsController : BaseController
    {
        private SiteAlertsManager _siteAlertsManager;
        private UserContext _userContext;
        private SiteContext _siteContext;

        public SiteAlertsController(ILogger<SiteAlertsController> logger, SiteAlertsManager siteAlertsManager,
            IContextAccessor<UserContext> userContextAccessor, IContextAccessor<SiteContext> siteContextAccessor) : base(logger)
        {
            _siteAlertsManager = siteAlertsManager;
            _userContext = userContextAccessor.GetContext();
            _siteContext = siteContextAccessor.GetContext();
        }

        [Authorize]
        [HttpGet("/sys/console/sitealerts/view/{id}")]
        public IActionResult view(string id,[FromQuery] string version = null)
        {
            ViewData["alertId"] = id;

            var model = _siteAlertsManager.Get(id, version);

            version = version ?? model.VersionCode;

            var versionInfo = _siteAlertsManager.GetVersionInfo(model);

            ViewData["AlertVersionInfo"] = versionInfo;

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/Alerts/AlertView.cshtml",
                ViewModel = model
            });

        }

        [Authorize]
        [HttpGet("/sys/console/sitealerts/list")]
        public IActionResult List()
        {
            var alerts = _siteAlertsManager.GetSiteAlerts(_siteContext.SiteId)
                            .Where(x => x.Status != ContentStatus.Scratch);

            return PartialView("~/UI/Views/Console/Alerts/AlertList.cshtml", alerts);
        }

        [Authorize]
        [HttpGet("/sys/console/sitealerts/edit/{id}")]
        public async Task<IActionResult> Edit(string id,[FromQuery] string versionCode)
        {

            var model = await _siteAlertsManager.GetDraftAlertFromVersion(id, _userContext.UserId, versionCode);

            var versionInfo = _siteAlertsManager.GetVersionInfo(model);

            ViewData["AlertVersionInfo"] = versionInfo;

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Console/Alerts/AlertDesign.cshtml",
                ViewModel = model,
                Editable = true
            });
        }

        [Authorize]
        [HttpGet("/sys/console/sitealerts/create")]
        public async Task<IActionResult> Create()
        {
            //if (!_blogSecurity.AuthorizeForCreate())
            //    return Unauthorized();
            
            var model = await _siteAlertsManager.CreateSiteAlertAsync(_userContext.UserId, _siteContext.SiteId);

            var versionInfo = _siteAlertsManager.GetVersionInfo(model);

            ViewData["AlertVersionInfo"] = versionInfo;

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "~/UI/Views/Console/Alerts/AlertDesign.cshtml",
                ViewModel = model,
                Editable = true
            });
            
        }

        [Authorize]
        [HttpDelete("/sys/console/sitealerts/delete/{id}")]
        public IActionResult Delete(string id)
        {
            _siteAlertsManager.Delete(id);

            return this.List();
        }
        
        [Authorize]
        [HttpPost("/sys/console/sitealerts/save")]
        public async Task<IActionResult> Save(SiteAlert model, [FromForm] bool ShouldPublish = false, [FromForm] string NewVersionLabel = null)
        {
            if (model != null && ModelState.IsValid)
            {
                model = await _siteAlertsManager.UpdateSiteAlertAsync(model, ShouldPublish, NewVersionLabel);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }
        
        [HttpGet("/sys/public/sitealerts/display")]
        public IActionResult Display(string alertId, string version = null)
        {
            ViewData["alertId"] = alertId;
            ViewData["ModalCssClass"] = "site-alert-modal";
            //set icon for the SiteAlerts
            ViewData["ModalIconClass"] = "fa fa-exclamation-circle";

            var model = _siteAlertsManager.Get(alertId);
            
            version = version ?? model.VersionCode;

            var versionInfo =_siteAlertsManager.GetVersionInfo(model);

            return this.PartialContentView(new ContentBindings(versionInfo)
            {
                ViewPath = "/UI/Views/Public/Alerts/SiteAlertItem.cshtml",
                ViewModel = model
            });
           
        }


    }
}
