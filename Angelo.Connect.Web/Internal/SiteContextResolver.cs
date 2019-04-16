using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Angelo.Common.Mvc.Saas;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using AutoMapper;
using AutoMapper.Extensions;
using Angelo.Connect.Extensions;
using System.Text;

namespace Angelo.Connect.Web
{
    public class SiteContextResolver : MemoryCacheTenantResolver<SiteContext>
    {
        private IEnumerable<Site> _sites;
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private SiteTemplateManager _templateManager;
        private IOptions<SiteOptions> _siteOptions;
        private ILogger _logger;

        public SiteContextResolver
        (
            SiteManager siteManager, 
            SiteTemplateManager templateManager,
            ClientManager clientManager,
            IOptions<SiteOptions> siteOptions,
            IMemoryCache cache,
            ILoggerFactory loggerFactory

        ) : base(cache, loggerFactory)
        {
            _siteOptions = siteOptions;
            _siteManager = siteManager;
            _clientManager = clientManager;
            _templateManager = templateManager;

            _logger = loggerFactory.CreateLogger("SiteContextResolver");           
        }

        protected override string GetContextIdentifier(HttpContext context)
        {
            return context.Request.Host.Value.ToLower();
        }

        protected override IEnumerable<string> GetTenantIdentifiers(TenantContext<SiteContext> tenantContext)
        {
            var domains = _siteManager.GetDomainsAsync(tenantContext.Tenant.SiteId).Result;

            return domains.Select(x => x.DomainKey);
        }

        protected override async Task<TenantContext<SiteContext>> ResolveAsync(HttpContext httpContext)
        {
            string domainKey = GetContextIdentifier(httpContext);
            Site site = _siteManager.GetByDomainKeyAsync(domainKey).Result;

            TenantContext<SiteContext> tenantContext = null;

            if (site != null)
            {
                var siteContext = Mapper.Map<SiteContext>(site);
                var defaultDomain = await _siteManager.GetDefaultDomainAsync(site.Id);

                // used to differentiate auth cookies between clients
                siteContext.Options = GetSiteOptions(site);
                siteContext.AuthCookieName = GetCookieName(site.Client);
                siteContext.ProductContext = _clientManager.GetProductContextAsync(site.ClientProductAppId).Result;
                siteContext.Template = _templateManager.GetTemplate(site.SiteTemplateId);
                siteContext.Theme = siteContext.Template.Themes.FirstOrDefault(x => x.Id == site.ThemeId);
                siteContext.DefaultDomain = defaultDomain.DomainKey;
                siteContext.DefaultUrl = BuildDefaultUrl(httpContext, defaultDomain);
                siteContext.Published = site.Published;
                siteContext.SiteSettings = await _siteManager.GetSiteSettingsAsync(site.Id);

                if (siteContext.Theme == null)
                {
                    siteContext.Theme = siteContext.Template.Themes.FirstOrDefault(x => x.IsDefault);
                }

                tenantContext = new TenantContext<SiteContext>(siteContext);
            }

            return tenantContext;
        }
    
        protected override MemoryCacheEntryOptions CreateCacheEntryOptions()
        {
            return base.CreateCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(3));
        }
        
        private Dictionary<string, string> GetSiteOptions(Site site)
        {
            var options = new Dictionary<string, string>();

            // get any settings from config file
            foreach(var item in _siteOptions.Value.Where(x => x.SiteKey == site.TenantKey))
            {
                options.Add(item.Key, item.Value);
            }

            // merge in db settings for site
            var settings = _siteManager.GetSiteSettingsAsync(site.Id).Result;
            foreach (var item in settings)
            {
                options.Add(item.FieldName, item.Value);
            }

            return options;
        }

        private string BuildDefaultUrl(HttpContext httpContext, SiteDomain defaultDomain)
        {
            // TODO: Extend SiteDomain object with UseHttps rather than checking the request
            var host = defaultDomain.DomainKey;
            var protocol = httpContext.Request.IsHttps ? "https://" : "http://";

            if (host.EndsWith("/"))
            {
                host = host.Substring(0, host.Length - 1);
            }

            return protocol + host;
        }

        private string GetCookieName(Client client)
        {
            // normalize client id for use in cookie
            var normalizedClientId = client.Id.ToLower().Replace("-", "");

            return $"connect.auth.{normalizedClientId}";
        }
    }
}
