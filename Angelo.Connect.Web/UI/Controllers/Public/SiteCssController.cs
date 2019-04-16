using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Angelo.Connect.Extensions;
using Angelo.Connect.Models;

using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Security.Services;

namespace Angelo.Connect.Web.UI.Controllers.Public
{
    public class SiteCssController : Controller
    {
        private SiteManager _siteManager;
        private SiteContextAccessor _siteContextAccessor;
        private PageSecurityService _pageSecurity;

        public SiteCssController(SiteManager siteManager, PageSecurityService pageSecurity, SiteContextAccessor siteContextAccessor)
        {
            _siteManager = siteManager;
            _siteContextAccessor = siteContextAccessor;
            _pageSecurity = pageSecurity;
        }

        [HttpGet, Route("/sys/site/css/dialog")]
        public async Task<IActionResult> SiteCssDialog()
        {
            var siteContext = _siteContextAccessor.GetContext();

            var cssSetting = await _siteManager.GetSiteSettingAsync(siteContext.SiteId, SiteSettingKeys.SITE_CSS);

            return PartialView("/UI/Views/Public/SitePage/StyleSheetModal.cshtml", cssSetting);
        }

        [HttpPost, Route("/sys/site/css")]
        public async Task<ActionResult> SaveCssSetting([FromForm] string css)
        {
            var siteContext = _siteContextAccessor.GetContext();

            if (siteContext == null)
                return BadRequest("Invalid Tenant");

            await _siteManager.SaveSiteSettingAsync(siteContext.SiteId, SiteSettingKeys.SITE_CSS, css);

            return Ok();
        }

        [HttpGet, Route("/sys/site/css")]
        public async Task<ActionResult> WriteCss()
        {
            var siteContext = _siteContextAccessor.GetContext();
            var cssSetting = await _siteManager.GetSiteSettingAsync(siteContext.SiteId, SiteSettingKeys.SITE_CSS);

            var cssText = cssSetting?.Value ?? "/* empty */";
            var cssBytes = System.Text.ASCIIEncoding.ASCII.GetBytes(cssText);

            return File(cssBytes, "text/css");
        }

    }
}
