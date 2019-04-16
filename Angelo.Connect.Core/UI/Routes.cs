using System;
using System.Text;
using System.Text.Encodings.Web;
using System.Reflection;
using System.Linq;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Models;

namespace Angelo.Connect.UI
{
    public class Routes
    {
        private ConnectCoreOptions _coreOptions;
        private SiteManager _siteManager;
        private IContextAccessor<AdminContext> _adminContextAccessor;
        private IContextAccessor<SiteContext> _siteContextAccessor;

        public Routes
        (
            IContextAccessor<AdminContext> adminContextAccessor, 
            IContextAccessor<SiteContext> siteContextAccessor, 
            SiteManager siteManager,
            ConnectCoreOptions coreOptions
        )
        {
            _adminContextAccessor = adminContextAccessor;
            _siteContextAccessor = siteContextAccessor;
            _siteManager = siteManager;
            _coreOptions = coreOptions;
        }

        public string Account(string action)
        {
            return Account(action, null);
        }

        public string Account(string action, object queryParams)
        {
            return BuildRoute("/sys/account", action, queryParams);
        }

        public string ClientAdmin()
        {
            return SiteAdmin(null, null);
        }

        public string ClientAdmin(string action)
        {
            return ClientAdmin(action, null);
        }

        public string ClientAdmin(string action, object queryParams)
        {
            var client = _adminContextAccessor.GetContext()?.ClientContext?.Client;

            if (client == null)
                throw new ArgumentNullException("Cannot create route from a null client context");

            var basePath =  $"/sys/clients/{client.TenantKey}/admin";

            return BuildRoute(basePath, action, queryParams);
        }

        public string ClientApi(string action)
        {
            return ClientApi(action, null);
        }

        public string ClientApi(string action, object queryParams)
        {
            var client = _adminContextAccessor.GetContext()?.ClientContext?.Client;

            if (client == null)
                throw new ArgumentNullException("Cannot create route from a null client context");

            var basePath = $"/sys/clients/{client.TenantKey}/api";

            return BuildRoute(basePath, action, queryParams);
        }

        public string SiteAdmin()
        {
            return SiteAdmin(null, null);
        }

        public string SiteAdmin(string action)
        {
            return SiteAdmin(action, null);
        }

        public string SiteAdmin(string action, object queryParams)
        {
            var site = _adminContextAccessor.GetContext()?.SiteContext?.Site;

            if (site == null)
                throw new ArgumentNullException("Cannot create route from a null site context");

            var basePath = $"/sys/sites/{site.TenantKey}/admin";

            return BuildRoute(basePath, action, queryParams);
        }

        public string SiteApi(string action)
        {
            return SiteApi(action, null);
        }

        public string SiteApi(string action, object queryParams)
        {
            var site = _adminContextAccessor.GetContext()?.SiteContext?.Site;

            if (site == null)
                throw new ArgumentNullException("Cannot create route from a null site context");

            var basePath = $"/sys/sites/{site.TenantKey}/api";

            return BuildRoute(basePath, action, queryParams);
        }

        public string CorpAdmin(string action)
        {
            return CorpAdmin(action, null);
        }

        public string CorpAdmin(string action, object queryParams)
        {
            return BuildRoute("/sys/corp/admin", action, queryParams);
        }

        public string CorpApi(string action)
        {
            return CorpApi(action, null);
        }

        public string CorpApi(string action, object queryParams)
        {
            return BuildRoute("/sys/corp/api", action, queryParams);
        }


        public string PublicUrl(string route = null, object queryParams = null)
        {
            var adminContext = _adminContextAccessor.GetContext();
           
            if(adminContext?.SiteContext != null)
            {
                return PublicUrl(adminContext.SiteContext.Site, route);
            }

            // else
            // NOTE: Should not ever hit here since SiteAdminContext defaults to SiteContext
            //       but putting code here anyway in case that gets changed
            var siteContext = _siteContextAccessor.GetContext();
            var site = _siteManager.GetByIdAsync(siteContext.SiteId).Result;

            return PublicUrl(site, route);
        }

        public string PublicUrl(Site site, string route = null, object queryParams = null)
        {
            var url = _coreOptions.UseHttpsForAbsoluteUris ? "https://" : "http://";
            var defaultDomain = _siteManager.GetDefaultDomainAsync(site.Id).Result;

            if (!route.StartsWith("/"))
                route = "/" + route;

            url += defaultDomain.DomainKey;
            url += route;

            if (queryParams != null)
                url += BuildQueryString(queryParams);

            return url;
        }

        private static string BuildRoute(string baseRoute, string relativeRoute, object queryParams)
        {
            var route = baseRoute;

            if (!string.IsNullOrEmpty(relativeRoute))
                route += "/" + relativeRoute.ToLower().Trim();

            if (queryParams != null)
                route += BuildQueryString(queryParams);

            return route;
        }

        private static string BuildQueryString(object parameters)
        {
            var sb = new StringBuilder();
            var encoder = UrlEncoder.Default;

            foreach (PropertyInfo prop in parameters.GetType().GetProperties())
            {
                var value = Convert.ToString(prop.GetValue(parameters)).Trim();

                sb.Append(encoder.Encode(prop.Name));
                if (!string.IsNullOrEmpty(value))
                {
                    sb.Append("=" + encoder.Encode(value));
                }
                sb.Append("&");
            }

            var result = sb.ToString();

            if (result.Length > 0)
                result = "?" + result.Substring(0, result.Length - 1);

            return result;
        }

    }
}
