using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Models;
using Angelo.Connect.Services;

namespace Angelo.Connect.Web.UI.Controllers
{ 
    public class SiteControllerBase : AdminController
    { 
        protected Site Site { get; private set; }

        public SiteControllerBase(SiteAdminContextAccessor siteContextAccessor,  ILogger logger) : base(logger)
        {
            Site = siteContextAccessor.GetContext()?.Site;

            // fail now since all subsequent actions & security depend on knowing which site we're operating on 
            if (Site == null)
            {
                logger.LogError($"SiteAdminContext.Site was null during initialization of {nameof(SiteControllerBase)}");

                throw new ArgumentNullException("SiteAdminContext.Site cannot be null for Site Admin Controllers to function properly.");
            }
        }


        public IActionResult RedirectToSiteAction(string action)
        {
            action = action.ToLower().Trim();

            return Redirect($"/sys/sites/{Site.TenantKey}/admin/{action}");
        }

    }
}
