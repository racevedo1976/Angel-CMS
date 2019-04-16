using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Web.Config;
using Angelo.Connect.Services;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web
{
    public class SiteAdminContextAccessor : IContextAccessor<SiteAdminContext>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private SiteManager _siteManager;
        private SiteAdminContext _currentContext;
        private ClientManager _clientManager;
        private ConnectCoreOptions _coreOptions;

        public SiteAdminContextAccessor(IHttpContextAccessor httpContextAccessor, SiteManager siteManager, ClientManager clientManager
            //, ConnectCoreOptions coreOptions
            )
        {
            _httpContextAccessor = httpContextAccessor;
            _siteManager = siteManager;
            _clientManager = clientManager;
            //_coreOptions = coreOptions;
        }

        public SiteAdminContext GetContext()
        {
            if (_currentContext == null)
            {
                _currentContext = GetFromRequestContext().Result;

                if(_currentContext == null)
                {
                    _currentContext = GetFromCurrentSiteContext().Result;
                }
            }

            return _currentContext;
        }

        private async Task<SiteAdminContext> GetFromRequestContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var route = httpContext.Request.Path.ToString();
            var query = httpContext.Request.Query;

            SiteAdminContext context = null;
            Site currentSite = null;

            if (route.StartsWith("/sys/sites/", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(route, @"/sys/sites/(?<tenant>[^\s/]*)");
                if (match.Success)
                {
                    var tenant = match.Groups["tenant"].Value;

                    currentSite = await _siteManager.GetByTenantKeyAsync(tenant);
                }
            }
            else if (query.ContainsKey("siteId"))
            {
                var siteId = query["siteId"];

                currentSite = await _siteManager.GetByIdAsync(siteId);
            }


            if(currentSite != null)
            {
                context = new SiteAdminContext(currentSite, _siteManager, _clientManager);
            }


            return context;
        }

        private async Task<SiteAdminContext> GetFromCurrentSiteContext()
        {
            var siteContext = _httpContextAccessor.HttpContext.GetTenant<SiteContext>(); 
            var currentSite = await _siteManager.GetByIdAsync(siteContext.SiteId);

            return new SiteAdminContext(currentSite, _siteManager, _clientManager);
        }


    }
}
