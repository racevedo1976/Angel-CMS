using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

using Angelo.Common.Mvc.Saas;
using Angelo.Aegis.Configuration;
using Angelo.Identity;
using Angelo.Identity.Models;

namespace Angelo.Aegis.Internal
{
    public class AegisTenantResolver : ITenantResolver<AegisTenant>
    {
        private TenantManager _tenantManager;
        private SecurityPoolManager _poolManager;
        private ServerOptions _serverDefaults;
        private Dictionary<string, string> _siteTitles;
        
        public AegisTenantResolver
        (

            TenantManager tenantManager,
            SecurityPoolManager poolManager, 
            IOptions<ServerOptions> serverOptions
        )
        {
            _tenantManager = tenantManager;
            _poolManager = poolManager;
            _serverDefaults = serverOptions.Value;
        }

        public async Task<TenantContext<AegisTenant>> ResolveAsync(HttpContext httpContext)
        {           
            var tenantKey = GetTenantKeyFromRequest(httpContext);
            AegisTenant tenant = null;

            if (tenantKey != null)
            {
                tenant = await CreateAegisTenant(tenantKey);
            }
            else
            {
                tenant = CreateEmptyTenant();
            }


            return await Task.FromResult(new TenantContext<AegisTenant>(tenant));
        }

        //private string GetTenantKeyFromRequest(HttpContext context)
        //{
        //    var pattern = new Regex(@"\/([\w\-]+)");
        //    var currentPath = context.Request.Path.Value ?? "";
        //    string result = null;

        //    if (pattern.IsMatch(currentPath))
        //    {
        //        var matches = pattern.Matches(currentPath);

        //        result = matches[0].Groups[1].Value.ToLowerInvariant();
        //    }

        //    return result;
        //}

        private string GetTenantKeyFromRequest(HttpContext context)
        {
            var currentPath = string.Concat(context.Request.PathBase.Value ?? "", context.Request.Path.Value ?? "");
            var list = currentPath.ToLowerInvariant().Split('/');
            var index1 = Array.IndexOf(list, "auth");
            if ((index1 > -1) && (list.Length > (index1 + 1)))
            {
                return list[index1 + 1];
            }
            else
                return null;
        }

        private async Task<SecurityPool> GetPoolIdFromRequest(HttpContext httpContext)
        {
            var acrValues = httpContext.Request.Query["acr_values"].ToString();
            SecurityPool securityPool = null;

            if (!String.IsNullOrEmpty(acrValues))
            {
                var poolId = Regex.Match(acrValues, @"[tenant:][^\s]+").Value.Replace("tenant:", "");

                if (!String.IsNullOrEmpty(poolId))
                {
                    securityPool = await _poolManager.GetByIdAsync(poolId);
                }
            }

            return securityPool;
        }

        private async Task<AegisTenant> CreateAegisTenant(string tenantKey)
        {
            var tenant = await _tenantManager.GetByKeyAsync(tenantKey);
         
            if (tenant == null)
                return CreateEmptyTenant();

            // A normalized version of Tenant.Id is scope each auth requests per tenant
            var normalizedTenantId = tenant.Id.ToLower().Replace("-", "");

            // Initialize base context
            var aegisTenant = new AegisTenant()
            {
                TenantId = tenant.Id,
                TenantKey = tenant.Key,

                AuthSchemeInternal = "aegis.internal." + normalizedTenantId,
                AuthSchemeExternal = "aegis.external." + normalizedTenantId,       
            };


            // 
            // Configure Identity Server Connection for this Tenant
            //
            var connectClient = new ConnectClient();

            var signinRedirectUris = await _tenantManager.GetUrisAsync(tenantKey, TenantUriType.OidcSignin);
            var logoutRedirectUris = await _tenantManager.GetUrisAsync(tenantKey, TenantUriType.OidcPostLogout);

            connectClient.SetSigninRedirectUris(signinRedirectUris);
            connectClient.SetPostLogoutRedirectUris(logoutRedirectUris);

            aegisTenant.Clients = new List<IdentityServer4.Models.Client> { connectClient };


            //
            // Configure Password Options
            // TODO: Persist password options per client. (Using defaults for now).
            //

            aegisTenant.PasswordOptions = _serverDefaults.OpenId.Password;


            //
            // Configure External Auth Providers
            // TODO: Persist auth providers per client. (Using defaults for now).
            //

            aegisTenant.ProviderOptions = _serverDefaults.OpenId.Providers;

            if (aegisTenant.ProviderOptions?.Facebook != null)
                aegisTenant.ProviderOptions.Facebook.AuthScheme = "aegis.external.facebook." + tenantKey;

            if (aegisTenant.ProviderOptions?.Google != null)
                aegisTenant.ProviderOptions.Google.AuthScheme = "aegis.external.google." + tenantKey;

            if (aegisTenant.ProviderOptions?.Twitter != null)
                aegisTenant.ProviderOptions.Twitter.AuthScheme = "aegis.external.twitter." + tenantKey;

            if (aegisTenant.ProviderOptions?.Microsoft != null)
                aegisTenant.ProviderOptions.Microsoft.AuthScheme = "aegis.external.microsoft." + tenantKey;


            //
            // Configure Custom UI Elements
            // 

            aegisTenant.SiteTitle = tenant.OidcTitle;
            aegisTenant.SiteBanner = tenant.OidcBanner ?? "/img/default-banner.png";

            return await Task.FromResult(aegisTenant);
        }

        private AegisTenant CreateEmptyTenant()
        {
            return new AegisTenant
            {
                TenantKey = null,
                SiteTitle = "Aegis Identity Server",
                SiteBanner = "/img/default-banner.png",
                AuthSchemeInternal = "aegis.internal.empty",
                AuthSchemeExternal = "aegis.external.empty",
                Clients = new List<IdentityServer4.Models.Client>(),
                PasswordOptions = _serverDefaults.OpenId.Password,
                ProviderOptions = new ExternalProviders()
            };
        }
    }
}
